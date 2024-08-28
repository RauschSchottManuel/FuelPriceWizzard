using FuelPriceWizard.BusinessLogic.Modules.Enums;
using Microsoft.Extensions.Configuration;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Zeas");

            //dings recurring event mochn

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var service = FuelPriceSourceServiceFactory.GetFuelPriceSourceService(config);

            var result = service.FetchPricesByLocation(1, 0);
            var c = (IConfiguration)service.FetchPricesByLocationAndFuelType(1, 0, FuelType.Diesel);

            Console.WriteLine(result);
            Console.WriteLine(c.GetValue<string>("sui"));
        }
    }
}
