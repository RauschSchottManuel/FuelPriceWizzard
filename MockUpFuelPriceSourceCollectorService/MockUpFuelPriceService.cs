using FuelPriceWizard.BusinessLogic;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FuelPriceWizard.DataAccess;

namespace MockUpFuelPriceSourceImplementation
{
    public class MockUpFuelPriceService : BaseFuelPriceSourceService, IFuelPriceSourceService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public override Dictionary<string, Enums.FuelType> FuelTypeMapping => new();
        public override Enums.Currency Currency => Enums.Currency.EUR;

        public MockUpFuelPriceService(IConfiguration config, 
            HttpClient httpClient, 
            ILogger<MockUpFuelPriceService> logger, 
            IFuelTypeRepository fuelTypeRepository,
            ICurrencyRepository currencyRepository) 
            : base(config, fuelTypeRepository, currencyRepository)
        {
            _httpClient = httpClient;
            _logger = logger;
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
