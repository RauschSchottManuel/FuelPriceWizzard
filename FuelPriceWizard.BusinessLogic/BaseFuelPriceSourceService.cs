using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Util;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// <c>BaseFuelPriceSourceService</c> defines a base class which a collector service entry point class has to extend.
    /// This base class contains some common methods that are required for the collector service to work properly.
    /// </summary>
    public abstract class BaseFuelPriceSourceService<T>(IConfiguration configuration, ILogger<T> logger, IFuelTypeRepository fuelTypeRepository, ICurrencyRepository currencyRepository)
    {
        /// <summary>
        /// Stores the configuration of the appsettings.json and the custom appsettings.<CustomCollectorServiceClassName>.json
        /// configuration files.
        /// </summary>
        public IConfiguration Configuration { get; } = configuration;

        /// <summary>
        /// Stores the logger provided by Dependency Injection
        /// </summary>
        public ILogger<T> Logger { get; } = logger;

        /// <summary>
        /// Stores a FuelTypeRepository provided by Dependency Injection
        /// </summary>
        public IFuelTypeRepository FuelTypeRepository { get; } = fuelTypeRepository;

        /// <summary>
        /// Stores a CurrencyRepository provided by Dependency Injection
        /// </summary>
        public ICurrencyRepository CurrencyRepository { get; } = currencyRepository;

        /// <summary>
        /// Defines the mapping between <see cref="BusinessLogic.Modules.Enums.FuelType"/> and implementation specific FuelType 
        /// string.
        /// </summary>
        public abstract Dictionary<string, Enums.FuelType> FuelTypeMapping { get; }

        protected virtual TimeSpan DefaultCacheValidityTimeSpan { get; set; } = new TimeSpan(2, 0, 0);
        /// <summary>
        /// Caches FuelTypes stored in the database.
        /// </summary>
        public required Cashed<FuelType> CashedFuelTypes { get; set; }

        /// <summary>
        /// Defines the default currency for the price value.
        /// </summary>
        public abstract Enums.Currency Currency { get; }
        public Currency? CurrencyObject => CashedCurrencies.Get().FirstOrDefault(c => c.Abbreviation == Currency.ToString());

        /// <summary>
        /// Caches Currencies stored in the database.
        /// </summary>
        public required Cashed<Currency> CashedCurrencies { get; set; }


        /// <summary>
        /// The method <c>GetFetchSettingsSection</c> returns the specific <c>FetchSettings</c> configuration section 
        /// </summary>
        /// <returns>An object of type <see cref="IConfigurationSection"/></returns>
        public IConfigurationSection GetFetchSettingsSection() =>
            this.Configuration.GetSection("FetchSettings");

        /// <summary>
        /// The method <c>Setup</c> is used for additional setup of the collector like initializing variables.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task Setup()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            CashedCurrencies = new Cashed<Currency>(DefaultCacheValidityTimeSpan, () =>
            {
                return this.CurrencyRepository.GetAllAsync().Result;
            });

            CashedFuelTypes = new Cashed<FuelType>(DefaultCacheValidityTimeSpan, () =>
            {
                return this.FuelTypeRepository.GetAllAsync().Result;
            });
        }

        protected FuelType MapToFuelType(string? value)
        {
            var mappingExists = FuelTypeMapping.TryGetValue(value ?? string.Empty, out var typeToFetch);

            if (!mappingExists)
            {
                this.Logger.LogError("No FuelTypeMapping found for value {FuelTypeValue}", value);
                return new FuelType
                {
                    DisplayValue = value ?? string.Empty,
                };
            }

            return this.CashedFuelTypes.Get().FirstOrDefault(e => e.DisplayValue == typeToFetch.ToString())
                ?? new FuelType()
                {
                    DisplayValue = string.Empty
                };
        }

        protected string MapFromFuelType(Enums.FuelType fuelType)
        {
            return FuelTypeMapping.FirstOrDefault(e => e.Value == fuelType).Key;
        }
    }
}
