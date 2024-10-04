using FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                .Build();

            var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            //Load impl. assemblies
            var services = FuelPriceSourceServiceFactory.GetFuelPriceSourceServices(config);
            var tasks = new List<Task>();

            foreach (var service in services)
            {
                var serviceClassName = service.GetType().GetGenericArguments().First();

                var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

                if(fetchSettings is null)
                {
                    logger.Error("No FetchSettings specified in appsettings.{0}.json or GetFetchSettingsSection() not implemented! Skipping creation of task for {0}", serviceClassName);
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
                    logger.Error("Invalid fetch interval specified! Skipping creation of task for {0}", serviceClassName);
                    continue;
                }


                logger.Information("Starting task for service {0}", serviceClassName);

                tasks.Add(PeriodicTask(async () =>
                {
                    //await service.FetchPricesByLocationAsync(0, 1);    //TODO: impl. location config dings
                    var dieselPrice = await service.FetchPricesByLocationAndFuelTypeAsync(48.287689M, 14.107360M, FuelType.Diesel);

                    var prices = await service.FetchPricesByLocationAsync(48.287689M, 14.107360M);

                    foreach (var price in prices)
                    {
                        logger.Debug("Price {0} {1}", price.FuelType.DisplayValue, price.Value);
                    }

                }, interval, fetchSettings.ExcludedWeekdays, fetchSettings.StartNextFullHour));
            }

            Console.ReadKey();
        }

        public static async Task PeriodicTask(Func<Task> func, TimeSpan interval, List<DayOfWeek> excludedWeekdays, bool waitForFullHour = false, CancellationToken cancellationToken = default)
        {
            using PeriodicTimer timer = new PeriodicTimer(interval);

            //Calculate time to full hour and wait
            if (waitForFullHour)
            {
                await WaitForNextFullHour();
            }

            while(true)
            {
                while (excludedWeekdays.Contains(DateTime.Now.DayOfWeek))
                {
                    await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now);
                }

                await func();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

        private static async Task WaitForNextFullHour()
        {
            DateTime dateTime = DateTime.Now;

            //Change DateTime to full hour
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            dateTime = dateTime.AddMinutes(-dateTime.Minute);
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);

            //Add 1 hour, to reach next full hour
            dateTime = dateTime.AddHours(1);

            await Task.Delay(dateTime - DateTime.Now);
        }
    }
}
