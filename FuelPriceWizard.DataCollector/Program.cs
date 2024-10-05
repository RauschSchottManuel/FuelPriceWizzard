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

            //Load impl. assemblies
            var services = FuelPriceSourceServiceFactory.GetFuelPriceSourceServices(config, logger);
            var tasks = new List<Task>();

            foreach (var service in services)
            {
                var serviceClassName = service.GetType().GetGenericArguments()[0];

                var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

                if(fetchSettings is null)
                {
                    contextLogger.Error("No FetchSettings specified in appsettings.{0}.json or GetFetchSettingsSection() not implemented!"
                        + " Skipping creation of task for instance {0}",
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
                    contextLogger.Error("Invalid fetch interval specified! Skipping creation of task for instance {0}", serviceClassName);
                    continue;
                }


                contextLogger.Information("Creating task for instance {0}", serviceClassName);

                tasks.Add(PeriodicTask(contextLogger, async () =>
                {
                    //await service.FetchPricesByLocationAsync(0, 1);    //TODO: impl. location config dings
                    //var dieselPrice = await service.FetchPricesByLocationAndFuelTypeAsync(48.287689M, 14.107360M, FuelType.Diesel);

                    var prices = await service.FetchPricesByLocationAsync(48.287689M, 14.107360M);

                    foreach (var price in prices)
                    {
                        contextLogger.Debug("Price {0} {1}", price.FuelType.DisplayValue, price.Value);
                    }

                }, interval, fetchSettings.ExcludedWeekdays, fetchSettings.StartNextFullHour));
            }

            Console.ReadKey();
        }

        private static async Task PeriodicTask(ILogger logger, Func<Task> func,
            TimeSpan interval, List<DayOfWeek> excludedWeekdays, bool waitForFullHour = false,
            CancellationToken cancellationToken = default)
        {
            logger.Information("Preparing task ...");

            using PeriodicTimer timer = new(interval);

            //Calculate time to full hour and wait
            if (waitForFullHour)
            {
                await WaitForNextFullHour(logger);
            }

            logger.Information("Finished preparing task!");
            logger.Information("Starting task ...");

            while (!cancellationToken.IsCancellationRequested)
            {
                while (excludedWeekdays.Contains(DateTime.Now.DayOfWeek))
                {
                    logger.Information("Fetch settings are configured to not run on the following days: {ExcludedDays}."
                        + " Next execution try on {NextTryDate:dd.MM.yyyy HH:mm}",
                        excludedWeekdays, DateTime.Today.AddDays(1));
                    await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now, cancellationToken);
                }

                await func();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

        private static async Task WaitForNextFullHour(ILogger logger)
        {
            DateTime dateTime = DateTime.Now;

            //Change DateTime to full hour
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            dateTime = dateTime.AddMinutes(-dateTime.Minute);
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);

            //Add 1 hour, to reach next full hour
            dateTime = dateTime.AddHours(1);

            logger.Information("Fetch settings are configured to start on full hour. First execution on {NextFullHour:dd.MM.yyyy HH:mm}!",
                dateTime);

            await Task.Delay(dateTime - DateTime.Now);
        }
    }
}
