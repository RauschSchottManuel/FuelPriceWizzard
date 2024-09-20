using FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Zeas");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                .Build();

            //Load impl. assemblies
            var services = FuelPriceSourceServiceFactory.GetFuelPriceSourceServices(config);
            var tasks = new List<Task>();

            foreach (var service in services)
            {
                var fetchSettings = service.GetFetchSettingsSection().Get<FetchSettings>();

                var interval = fetchSettings.IntervalUnit switch
                {
                    FetchSettings.TimeUnit.Second => new TimeSpan(0, 0, fetchSettings.IntervalValue),
                    FetchSettings.TimeUnit.Minute => new TimeSpan(0, fetchSettings.IntervalValue, 0),
                    FetchSettings.TimeUnit.Hour => new TimeSpan(fetchSettings.IntervalValue, 0, 0),
                    _ => throw new ArgumentException("Invalid fetch interval unit specified.")
                };

                tasks.Add(PeriodicTask(async () =>
                {
                    service.FetchPricesByLocation(0, 1);    //TODO: impl. location config dings
                    service.FetchPricesByLocationAndFuelType(0, 1, FuelType.Diesel);
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
