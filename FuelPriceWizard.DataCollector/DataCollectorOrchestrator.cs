using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FuelPriceWizard.DataCollector
{
    public class DataCollectorOrchestrator(ILogger<DataCollectorOrchestrator> orchestratorLogger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        IFuelTypeRepository fuelTypeRepository) : IDataCollectorOrchestrator
    {
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

        private Func<ILogger, IFuelPriceSourceService, Task> CollectMethod() =>
            async (logger, service) =>
            {
                var prices = await service.FetchPricesByLocationAsync(48.287689M, 14.107360M);

                foreach (var price in prices)
                {
                    logger.LogDebug("Price {FuelTypeDisplayValue} {FuelPrice}{Currency}", price.FuelType.DisplayValue, price.Value, price.Currency.Symbol);
                }
            };
    }
}
