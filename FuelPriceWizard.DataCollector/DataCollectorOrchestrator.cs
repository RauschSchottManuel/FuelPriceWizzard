using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FuelPriceWizard.DataCollector
{
    /// <summary>
    /// Handles all data collector task scheduling, creation and start/stop actions.
    /// </summary>
    /// <param name="orchestratorLogger"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="fuelTypeRepository"></param>
    public class DataCollectorOrchestrator(ILogger<DataCollectorOrchestrator> orchestratorLogger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        IFuelTypeRepository fuelTypeRepository,
        IGasStationRepository gasStationRepository,
        IPriceRepository priceRepository) : IDataCollectorOrchestrator
    {
        private readonly object _insertLock = new object();
        public ILogger<DataCollectorOrchestrator> Logger { get; } = orchestratorLogger;
        public IConfiguration Configuration { get; } = configuration;
        public ILoggerFactory LoggerFactory { get; } = loggerFactory;
        public IFuelTypeRepository FuelTypeRepository { get; } = fuelTypeRepository;
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

            // Create a fresh set of tasks based on the updated configuration
            var newTasks = this.CreateTasks().ToDictionary(t => t.GetGenericType());

            // Determine which collectors have been removed
            var removedTasks = existingTasks.Keys.Except(newTasks.Keys);
            foreach (var taskName in removedTasks)
            {
                var task = existingTasks[taskName];
                await task.StopAsync();
                task.Dispose();
                this.Logger.LogInformation("Stopped collector {Collector}", taskName);
            }

            // Start tasks that have been newly added
            var addedTasks = newTasks.Keys.Except(existingTasks.Keys);
            foreach (var taskName in addedTasks)
            {
                var task = newTasks[taskName];
                _ = task.Start(this.CollectMethod());
                this.Logger.LogInformation("Started collector {Collector}", taskName);
            }

            // Dispose tasks created during reload for collectors that already existed
            var unchangedTasks = newTasks.Keys.Intersect(existingTasks.Keys);
            foreach (var taskName in unchangedTasks)
            {
                newTasks[taskName].Dispose();
            }

            // Update the list of running tasks to include kept and newly added tasks
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
                var gasStations = (await gasStationRepository.GetAllAsync())
                                    .Where(g => g.IsActive)
                                    .ToList();

                var tasks = new List<Task>();

                //Fetch all existing gas stations and iterate to fetch prices
                foreach (var gasStation in gasStations)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var prices = await service.FetchPricesByLocationAsync(Convert.ToDecimal(gasStation.Address!.Lat), Convert.ToDecimal(gasStation.Address!.Long));

                        foreach (var price in prices)
                        {
                            price.GasStationId = gasStation.Id;
                            lock (_insertLock)
                            {
                                price.FetchedAt = DateTime.UtcNow;
                                var inserted = priceRepository.InsertAsync(price).Result;
                                //TODO: add refs to currency, fueltype and gasstation to result of insert and log result
                                //logger.LogDebug("Price {FuelTypeDisplayValue} {FuelPrice}{Currency}", price.FuelType.DisplayValue, price.Value, price.Currency.Symbol);
                            }

                            //TODO: Delete eventually
                            logger.LogDebug("Price {FuelTypeDisplayValue} {FuelPrice} {Currency}", price.FuelTypeId, price.Value, price.CurrencyId);
                        }
                    }));
                }

                Task.WaitAll([.. tasks]);
            };
    }
}
