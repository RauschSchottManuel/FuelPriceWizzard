using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Util;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// Base class that a collector service entry point must extend.
    /// Provides caching, fuel-type mapping, and configuration helpers.
    /// </summary>
    public abstract class BaseFuelPriceSourceService<T>(IConfiguration configuration, ILogger<T> logger, IFuelTypeRepository fuelTypeRepository, ICurrencyRepository currencyRepository)
    {
        public IConfiguration Configuration { get; } = configuration;
        public ILogger<T> Logger { get; } = logger;
        public IFuelTypeRepository FuelTypeRepository { get; } = fuelTypeRepository;
        public ICurrencyRepository CurrencyRepository { get; } = currencyRepository;

        public abstract Dictionary<string, Enums.FuelType> FuelTypeMapping { get; }

        protected virtual TimeSpan DefaultCacheValidityTimeSpan { get; set; } = new TimeSpan(2, 0, 0);

        protected Cached<FuelType> CachedFuelTypes { get; } = new Cached<FuelType>(
            new TimeSpan(2, 0, 0),
            async () => await fuelTypeRepository.GetAllAsync());

        public abstract Enums.Currency Currency { get; }

        protected Cached<Currency> CachedCurrencies { get; } = new Cached<Currency>(
            new TimeSpan(2, 0, 0),
            async () => await currencyRepository.GetAllAsync());

        public async Task<Currency?> GetCurrencyObjectAsync() =>
            (await CachedCurrencies.GetAsync()).FirstOrDefault(c => c.Abbreviation == Currency.ToString());

        public IConfigurationSection GetFetchSettingsSection() =>
            this.Configuration.GetSection("FetchSettings");

        /// <summary>
        /// Override in subclasses for additional initialisation. Base implementation is a no-op.
        /// </summary>
        public virtual Task Setup() => Task.CompletedTask;

        /// <summary>
        /// Maps a collector-specific fuel-type string to the domain <see cref="FuelType"/>.
        /// Returns <c>null</c> when no mapping exists or the mapped type is not in the database.
        /// </summary>
        protected async Task<FuelType?> MapToFuelTypeAsync(string? value)
        {
            var mappingExists = FuelTypeMapping.TryGetValue(value ?? string.Empty, out var typeToFetch);

            if (!mappingExists)
            {
                this.Logger.LogError("No FuelTypeMapping found for value {FuelTypeValue}", value);
                return null;
            }

            return (await CachedFuelTypes.GetAsync()).FirstOrDefault(e => e.DisplayValue == typeToFetch.ToString());
        }

        protected string MapFromFuelType(Enums.FuelType fuelType)
        {
            return FuelTypeMapping.FirstOrDefault(e => e.Value == fuelType).Key;
        }
    }
}
