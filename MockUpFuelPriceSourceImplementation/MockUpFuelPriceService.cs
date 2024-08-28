using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.BusinessLogic.Modules.Enums;
using Microsoft.Extensions.Configuration;

namespace MockUpFuelPriceSourceImplementation
{
    public class MockUpFuelPriceService : IFuelPriceSourceService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public MockUpFuelPriceService(IConfiguration config, HttpClient httpClient)
        {
            _configuration = config;
            _httpClient = httpClient;
        }

        public object FetchPricesByLocation(int lat, int lon)
        {
            return _httpClient != null;
        }

        public object FetchPricesByLocationAndFuelType(int lat, int lon, FuelType fuelType, int maxReturnResults = 1)
        {
            return _configuration;
        }
    }
}
