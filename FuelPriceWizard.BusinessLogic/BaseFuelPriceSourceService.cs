using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// <c>BaseFuelPriceSourceService</c> defines a base class which a collector service entry point class has to extend.
    /// This base class contains some common methods that are required for the collector service to work properly.
    /// </summary>
    public abstract class BaseFuelPriceSourceService(IConfiguration configuration, IFuelTypeRepository fuelTypeRepository)
    {
        /// <summary>
        /// Stores the configuration of the appsettings.json and the custom appsettings.<CustomCollectorServiceClassName>.json
        /// configuration files.
        /// </summary>
        public IConfiguration Configuration { get; } = configuration;

        /// <summary>
        /// Stores a FuelTypeRepository provided by Dependency Injection
        /// </summary>
        public IFuelTypeRepository FuelTypeRepository { get; } = fuelTypeRepository;

        /// <summary>
        /// The method <c>GetFetchSettingsSection</c> returns the specific <c>FetchSettings</c> configuration section 
        /// </summary>
        /// <returns>An object of type <see cref="IConfigurationSection"/></returns>
        public IConfigurationSection GetFetchSettingsSection() =>
            this.Configuration.GetSection("FetchSettings");

        /// <summary>
        /// Defines the mapping between <see cref="BusinessLogic.Modules.Enums.FuelType"/> and implementation specific FuelType 
        /// string.
        /// </summary>
        public abstract Dictionary<string, Enums.FuelType> FuelTypeMapping { get; protected set; }

        protected async Task<FuelType> MapToFuelTypeAsync(string? value)
        {
            var mappingExists = FuelTypeMapping.TryGetValue(value ?? string.Empty, out var typeToFetch);

            if (!mappingExists)
            {
                //TODO: Handle non existing fueltype
                return default;
            }

            return await FuelTypeRepository.GetByDisplayValueAsync(typeToFetch.ToString());
        }

        protected string MapFromFuelType(Enums.FuelType fuelType)
        {
            return FuelTypeMapping.FirstOrDefault(e => e.Value == fuelType).Key;
        }
    }
}
