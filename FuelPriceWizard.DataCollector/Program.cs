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
            var collectorTasks = new List<IRepeatingTask<IFuelPriceSourceFacade>>();

            foreach (var service in services)
            {
                var serviceClassName = service.GetType().GetGenericArguments()[0];

                var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

                if(fetchSettings is null)
                {
                    contextLogger.Error("No FetchSettings specified in appsettings.{ServiceName}.json or GetFetchSettingsSection() not implemented!"
                        + " Skipping creation of task for instance {ServiceName}",
                        serviceClassName);
                    continue;
                }

                var interval = fetchSettings!.IntervalUnit switch
                {
                    FetchSettings.TimeUnit.Second => new TimeSpan(0, 0, fetchSettings.IntervalValue),
                    FetchSettings.TimeUnit.Minute => new TimeSpan(0, fetchSettings.IntervalValue, 0),
                    FetchSettings.TimeUnit.Hour => new TimeSpan(fetchSettings.IntervalValue, 0, 0),
                    _ => TimeSpan.Zero,
                };

                if(interval == TimeSpan.Zero)
                {
                    contextLogger.Error("Invalid fetch interval specified! Skipping creation of task for instance {ServiceName}", serviceClassName);
                    continue;
                }


                contextLogger.Information("Creating task for instance {ServiceName}", serviceClassName);

                var repeatingTask = new RepeatingTask<IFuelPriceSourceFacade>(contextLogger, interval, service,
                    fetchSettings.ExcludedWeekdays, fetchSettings.StartNextFullHour, CancellationToken.None);

                collectorTasks.Add(repeatingTask);

                contextLogger.Information("Finished creating task for instance {ServiceName}", serviceClassName);
            }

            foreach(var collectorTask in collectorTasks)
            {
                contextLogger.Information("Starting collector task {CollectorName}", collectorTask.GetGenericType());
                collectorTask.Start(async (service) =>
                {
                    var prices = await service.FetchPricesByLocationAsync(48.287689M, 14.107360M);

                    foreach (var price in prices)
                    {
                        contextLogger.Debug("Price {0} {1}", price.FuelType.DisplayValue, price.Value);
                    }
                });
            }

            Console.ReadKey();
        }
    }
}
