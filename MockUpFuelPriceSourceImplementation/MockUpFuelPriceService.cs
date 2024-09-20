using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.BusinessLogic.Modules.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MockUpFuelPriceSourceImplementation
{
    public class MockUpFuelPriceService : IFuelPriceSourceService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public MockUpFuelPriceService(IConfiguration config, HttpClient httpClient, ILogger<MockUpFuelPriceService> logger)
        {
            _configuration = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        public object FetchPricesByLocation(int lat, int lon)
        {
            return _httpClient != null;
        }

        public object FetchPricesByLocationAndFuelType(int lat, int lon, FuelType fuelType, int maxReturnResults = 1)
        {
            _logger.LogDebug("Test nein");
            return _configuration;
        }

        public IConfigurationSection GetFetchSettingsSection()
        {
            return _configuration.GetSection("FetchSettings");
        }
    }
}
