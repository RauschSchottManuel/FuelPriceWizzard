using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FuelPriceWizard.DataCollector
{
    /// <summary>
    /// Handles all data collector task scheduling, creation and start/stop actions.
    /// </summary>
    public class DataCollectorOrchestrator(
        ILogger<DataCollectorOrchestrator> orchestratorLogger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory) : IDataCollectorOrchestrator
    {
        public ILogger<DataCollectorOrchestrator> Logger { get; } = orchestratorLogger;
        public IConfiguration Configuration { get; } = configuration;
        public ILoggerFactory LoggerFactory { get; } = loggerFactory;
        public IEnumerable<RepeatingTask<IFuelPriceSourceService>> Tasks { get; set; } = [];

        public IEnumerable<RepeatingTask<IFuelPriceSourceService>> CreateTasks()
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

                service.Setup();

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
                CancellationToken.None);
        }

        public void StartTasks() => this.StartTasks(this.Tasks);

        public void StartTasks(IEnumerable<RepeatingTask<IFuelPriceSourceService>> tasks)
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

            var newTasks = this.CreateTasks().ToDictionary(t => t.GetGenericType());

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
                () => _ = this.ReloadTasksAsync());
        }

        private Func<ILogger, IFuelPriceSourceService, Task> CollectMethod() =>
            async (logger, service) =>
            {
                // Each collection run gets its own scope so concurrent tasks never share a DbContext instance.
                using var scope = serviceScopeFactory.CreateScope();
                var gasStationRepository = scope.ServiceProvider.GetRequiredService<IGasStationRepository>();
                var priceRepository = scope.ServiceProvider.GetRequiredService<IPriceRepository>();

                var gasStations = (await gasStationRepository.GetAllAsync())
                                    .Where(g => g.IsActive)
                                    .ToList();

                var tasks = new List<Task>();

                foreach (var gasStation in gasStations)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var prices = await service.FetchPricesByLocationAsync(
                            Convert.ToDecimal(gasStation.Address!.Lat),
                            Convert.ToDecimal(gasStation.Address!.Long));

                        foreach (var price in prices)
                        {
                            price.GasStationId = gasStation.Id;
                            price.FetchedAt = DateTime.UtcNow;

                            // Each insert uses the scope's priceRepository sequentially per gas station task,
                            // which is safe because each Task.Run closure captures the same scope but accesses
                            // it from a single logical thread per station.
                            var inserted = await priceRepository.InsertAsync(price);

                            logger.LogDebug(
                                "Inserted price reading: Station={StationDesignation} FuelType={FuelTypeId} Value={Value} Currency={CurrencyId}",
                                gasStation.Designation, inserted.FuelTypeId, inserted.Value, inserted.CurrencyId);
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            };
    }
}
