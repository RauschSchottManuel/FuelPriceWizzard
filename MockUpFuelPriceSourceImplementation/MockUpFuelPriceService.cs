using FuelPriceWizard.BusinessLogic;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.Domain.Models;
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

        public IConfigurationSection GetFetchSettingsSection()
        {
            return _configuration.GetSection("FetchSettings");
        }

        public Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true)
        {
            return Task.FromResult<IEnumerable<PriceReading>>([]);
        }

        public Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true)
        {
            _logger.LogDebug("Test nein");
            return Task.FromResult<IEnumerable<PriceReading>>([]);
        }
    }
}
