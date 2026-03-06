using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Util;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace FuelPriceWizard.DataCollector.Tests
{
    /// <summary>
    /// Concrete test implementation of the abstract BaseFuelPriceSourceService.
    /// Exposes protected methods as public so they can be tested.
    /// </summary>
    public class TestFuelPriceSourceService(
        IConfiguration configuration,
        ILogger<TestFuelPriceSourceService> logger,
        IFuelTypeRepository fuelTypeRepository,
        ICurrencyRepository currencyRepository)
        : BaseFuelPriceSourceService<TestFuelPriceSourceService>(configuration, logger, fuelTypeRepository, currencyRepository)
    {
        public override Dictionary<string, Enums.FuelType> FuelTypeMapping => new()
        {
            { "DIE", Enums.FuelType.Diesel },
            { "SUP", Enums.FuelType.Super }
        };

        public override Enums.Currency Currency => Enums.Currency.EUR;

        public FuelType CallMapToFuelType(string? value) => MapToFuelType(value);
        public string CallMapFromFuelType(Enums.FuelType fuelType) => MapFromFuelType(fuelType);
    }

    public class BaseFuelPriceSourceServiceTests
    {
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<ILogger<TestFuelPriceSourceService>> _loggerMock = new();
        private readonly Mock<IFuelTypeRepository> _fuelTypeRepoMock = new();
        private readonly Mock<ICurrencyRepository> _currencyRepoMock = new();

        private static readonly IEnumerable<FuelType> SomeFuelTypes =
        [
            new FuelType { Id = 1, DisplayValue = "Diesel", Abbreviation = "DIE" },
            new FuelType { Id = 2, DisplayValue = "Super", Abbreviation = "SUP" }
        ];

        private static readonly IEnumerable<Currency> SomeCurrencies =
        [
            new Currency { Id = 1, Name = "Euro", Abbreviation = "EUR", Symbol = "€" }
        ];

        private TestFuelPriceSourceService CreateService(
            IEnumerable<FuelType>? fuelTypes = null,
            IEnumerable<Currency>? currencies = null)
        {
            return new TestFuelPriceSourceService(
                _configMock.Object,
                _loggerMock.Object,
                _fuelTypeRepoMock.Object,
                _currencyRepoMock.Object)
            {
                CashedFuelTypes = new Cashed<FuelType>(
                    TimeSpan.FromHours(1),
                    () => fuelTypes ?? SomeFuelTypes),
                CashedCurrencies = new Cashed<Currency>(
                    TimeSpan.FromHours(1),
                    () => currencies ?? SomeCurrencies)
            };
        }

        [Fact]
        public async Task Setup_InitializesCashedFuelTypes()
        {
            _fuelTypeRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<string[]>()))
                .ReturnsAsync(SomeFuelTypes);
            _currencyRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<string[]>()))
                .ReturnsAsync(SomeCurrencies);

            var service = new TestFuelPriceSourceService(
                _configMock.Object, _loggerMock.Object,
                _fuelTypeRepoMock.Object, _currencyRepoMock.Object)
            {
                CashedFuelTypes = null!,
                CashedCurrencies = null!
            };

            await service.Setup();

            Assert.NotNull(service.CashedFuelTypes);
            Assert.NotEmpty(service.CashedFuelTypes.Get());
        }

        [Fact]
        public async Task Setup_InitializesCashedCurrencies()
        {
            _fuelTypeRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<string[]>()))
                .ReturnsAsync(SomeFuelTypes);
            _currencyRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<string[]>()))
                .ReturnsAsync(SomeCurrencies);

            var service = new TestFuelPriceSourceService(
                _configMock.Object, _loggerMock.Object,
                _fuelTypeRepoMock.Object, _currencyRepoMock.Object)
            {
                CashedFuelTypes = null!,
                CashedCurrencies = null!
            };

            await service.Setup();

            Assert.NotNull(service.CashedCurrencies);
            Assert.NotEmpty(service.CashedCurrencies.Get());
        }

        [Fact]
        public void MapToFuelType_ReturnsCorrectFuelType_WhenMappingExists()
        {
            var service = CreateService();

            var result = service.CallMapToFuelType("DIE");

            Assert.Equal("Diesel", result.DisplayValue);
        }

        [Fact]
        public void MapToFuelType_ReturnsEmptyFuelType_WhenMappingDoesNotExist()
        {
            var service = CreateService();

            var result = service.CallMapToFuelType("UNKNOWN");

            Assert.Equal("UNKNOWN", result.DisplayValue);
        }

        [Fact]
        public void MapFromFuelType_ReturnsCorrectKey_WhenMappingExists()
        {
            var service = CreateService();

            var result = service.CallMapFromFuelType(Enums.FuelType.Diesel);

            Assert.Equal("DIE", result);
        }

        [Fact]
        public void CurrencyObject_ReturnsCorrectCurrency()
        {
            var service = CreateService();

            var currency = service.CurrencyObject;

            Assert.NotNull(currency);
            Assert.Equal("EUR", currency.Abbreviation);
        }
    }
}
