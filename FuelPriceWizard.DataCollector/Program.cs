using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        protected Program() { }

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                .Build();

            using var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            var contextLogger = logger.ForContext<Program>();

            contextLogger.Information("DataCollector started!");

            var services = FuelPriceSourceServiceFactory.GetFuelPriceSourceServices(config, logger);
            var collectorTasks = new List<IRepeatingTask<IFuelPriceSourceService>>();

            foreach (var service in services)
            {
                var task = ConstructRepeatingTask(logger, service);

                if (task is null)
                    continue;

                collectorTasks.Add(task);

                contextLogger.Information("Finished creating task for instance {ServiceName}", service.GetType().GetGenericArguments()[0]);
            }

            foreach (var collectorTask in collectorTasks)
            {
                collectorTask.Start(CollectMethod());
            }

            Console.ReadKey();
        }

        private static RepeatingTask<IFuelPriceSourceService>? ConstructRepeatingTask(Serilog.Core.Logger logger, IFuelPriceSourceService service)
        {
            var contextLogger = logger.ForContext<Program>();

            var serviceClassName = service.GetType().GetGenericArguments()[0];

            var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

            if (fetchSettings is null)
            {
                contextLogger.Error("No FetchSettings specified in appsettings.{ServiceName}.json or GetFetchSettingsSection() not implemented!"
                    + " Skipping creation of task for instance {ServiceName}",
                    serviceClassName);
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
                contextLogger.Error("Invalid fetch interval specified! Skipping creation of task for instance {ServiceName}", serviceClassName);
                return null;
            }


            contextLogger.Information("Creating task for instance {ServiceName}", serviceClassName);

            return new RepeatingTask<IFuelPriceSourceService>(logger.ForContext(serviceClassName), interval, service,
                fetchSettings.ExcludedWeekdays, fetchSettings.StartNextFullHour, CancellationToken.None);
        }

        private static Func<ILogger, IFuelPriceSourceService, Task> CollectMethod() =>
            async (logger, service) =>
            {
                var prices = await service.FetchPricesByLocationAsync(48.287689M, 14.107360M);

                foreach (var price in prices)
                {
                    logger.Debug("Price {0} {1}", price.FuelType.DisplayValue, price.Value);
                }
            };
    }

}
