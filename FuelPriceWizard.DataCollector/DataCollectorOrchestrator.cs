using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FuelPriceWizard.DataCollector
{
    /// <summary>
    /// Handles all data collector task scheduling, creation and start/stop actions.
    /// Registered as Singleton — uses IServiceScopeFactory to create short-lived scopes for DB access.
    /// </summary>
    public class DataCollectorOrchestrator(ILogger<DataCollectorOrchestrator> orchestratorLogger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IHostApplicationLifetime hostApplicationLifetime) : IDataCollectorOrchestrator
    {
        public ILogger<DataCollectorOrchestrator> Logger { get; } = orchestratorLogger;
        public IConfiguration Configuration { get; } = configuration;
        public ILoggerFactory LoggerFactory { get; } = loggerFactory;
        public IEnumerable<RepeatingTask<IFuelPriceSourceService>> Tasks { get; set; } = [];

        public async Task<IEnumerable<RepeatingTask<IFuelPriceSourceService>>> CreateTasksAsync()
        {
            var serviceFactoryLogger = this.LoggerFactory.CreateLogger<FuelPriceSourceServiceFactory>();
            var services = FuelPriceSourceServiceFactory.GetFuelPriceSourceServices(this.Configuration, serviceFactoryLogger);
            var collectorTasks = new List<RepeatingTask<IFuelPriceSourceService>>();

            foreach (var service in services)
            {
                var task = ConstructRepeatingTask(service);

                if (task is null)
                    continue;

                collectorTasks.Add(task);

                await service.Setup();

                this.Logger.LogInformation("Finished creating task for instance {ServiceName}", service.GetType().GetGenericArguments()[0]);
            }

            this.Tasks = collectorTasks;

            return collectorTasks;
        }

        private RepeatingTask<IFuelPriceSourceService>? ConstructRepeatingTask(IFuelPriceSourceService service)
        {
            var serviceClassName = service.GetType().GetGenericArguments()[0];

            var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

            if (fetchSettings is null)
            {
                this.Logger.LogError("No FetchSettings specified in appsettings.{AppsettingsServiceName}.json or GetFetchSettingsSection() not implemented!"
                    + " Skipping creation of task for instance {ServiceName}",
                    serviceClassName, serviceClassName);
                return null;
            }

            var interval = fetchSettings!.IntervalUnit switch
            {
                FetchSettings.TimeUnit.Second => new TimeSpan(0, 0, fetchSettings.IntervalValue),
                FetchSettings.TimeUnit.Minute => new TimeSpan(0, fetchSettings.IntervalValue, 0),
                FetchSettings.TimeUnit.Hour => new TimeSpan(fetchSettings.IntervalValue, 0, 0),
                _ => TimeSpan.Zero,
            };

            if (interval == TimeSpan.Zero)
            {
                this.Logger.LogError("Invalid fetch interval specified! Skipping creation of task for instance {ServiceName}", serviceClassName);
                return null;
            }

            var serviceLogger = this.LoggerFactory.CreateLogger(serviceClassName);

            this.Logger.LogInformation("Creating task for instance {ServiceName}", serviceClassName);

            return new RepeatingTask<IFuelPriceSourceService>(
                serviceLogger, interval, service,
                fetchSettings.ExcludedWeekdays, fetchSettings.StartNextFullHour,
                hostApplicationLifetime.ApplicationStopping);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await CreateTasksAsync();
            StartTasksInternal(this.Tasks);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            foreach (var task in this.Tasks)
            {
                await task.StopAsync();
                task.Dispose();
            }
            this.Tasks = [];
        }

        private void StartTasksInternal(IEnumerable<RepeatingTask<IFuelPriceSourceService>> tasks)
        {
            foreach (var task in tasks)
            {
                _ = task.Start(this.CollectMethod());
            }
        }

        public async Task ReloadTasksAsync()
        {
            this.Logger.LogInformation("Reloading collector tasks due to configuration change ...");

            var existingTasks = this.Tasks.ToDictionary(t => t.GetGenericType());

            var newTasks = (await this.CreateTasksAsync()).ToDictionary(t => t.GetGenericType());

            var removedTasks = existingTasks.Keys.Except(newTasks.Keys);
            foreach (var taskName in removedTasks)
            {
                var task = existingTasks[taskName];
                await task.StopAsync();
                task.Dispose();
                this.Logger.LogInformation("Stopped collector {Collector}", taskName);
            }

            var addedTasks = newTasks.Keys.Except(existingTasks.Keys);
            foreach (var taskName in addedTasks)
            {
                var task = newTasks[taskName];
                _ = task.Start(this.CollectMethod());
                this.Logger.LogInformation("Started collector {Collector}", taskName);
            }

            var unchangedTasks = newTasks.Keys.Intersect(existingTasks.Keys);
            foreach (var taskName in unchangedTasks)
            {
                newTasks[taskName].Dispose();
            }

            this.Tasks = existingTasks
                .Where(kvp => unchangedTasks.Contains(kvp.Key))
                .Select(kvp => kvp.Value)
                .Concat(addedTasks.Select(a => newTasks[a]))
                .ToList();
        }

        public void WatchForConfigurationChanges()
        {
            Microsoft.Extensions.Primitives.ChangeToken.OnChange(
                () => this.Configuration.GetReloadToken(),
                () =>
                {
                    _ = this.ReloadTasksAsync().ContinueWith(
                        t => this.Logger.LogError(t.Exception, "Failed to reload collector tasks after configuration change."),
                        TaskContinuationOptions.OnlyOnFaulted);
                });
        }

        private Func<ILogger, IFuelPriceSourceService, Task> CollectMethod() =>
            async (logger, service) =>
            {
                IReadOnlyList<Domain.Models.GasStation> gasStations;
                using (var readScope = serviceScopeFactory.CreateScope())
                {
                    var gasStationRepo = readScope.ServiceProvider.GetRequiredService<IGasStationRepository>();
                    gasStations = (await gasStationRepo.GetAllAsync())
                        .Where(g => g.IsActive)
                        .ToList();
                }

                var tasks = gasStations.Select(gasStation => Task.Run(async () =>
                {
                    var lat = Convert.ToDecimal(gasStation.Address!.Lat);
                    var lon = Convert.ToDecimal(gasStation.Address!.Long);

                    logger.LogInformation(
                        "Collecting prices for gas station '{Name}' (ID {Id}) at ({Lat}, {Lon}) ...",
                        gasStation.Designation, gasStation.Id, lat, lon);

                    var prices = (await service.FetchPricesByLocationAsync(lat, lon)).ToList();

                    using var writeScope = serviceScopeFactory.CreateScope();
                    var priceRepo = writeScope.ServiceProvider.GetRequiredService<IPriceRepository>();

                    foreach (var price in prices)
                    {
                        price.GasStationId = gasStation.Id;
                        await priceRepo.InsertAsync(price);
                    }

                    logger.LogInformation(
                        "Inserted {Count} price reading(s) for gas station '{Name}' (ID {Id}).",
                        prices.Count, gasStation.Designation, gasStation.Id);
                }));

                await Task.WhenAll(tasks);
            };
    }
}
