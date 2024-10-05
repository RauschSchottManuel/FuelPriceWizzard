using Microsoft.Extensions.Configuration;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// <c>BaseFuelPriceSourceService</c> defines a base class which a collector service entry point class has to extend.
    /// This base class contains some common methods that are required for the collector service to work properly.
    /// </summary>
    public abstract class BaseFuelPriceSourceService(IConfiguration configuration)
    {
        /// <summary>
        /// Stores the configuration of the appsettings.json and the custom appsettings.<CustomCollectorServiceClassName>.json
        /// configuration files.
        /// </summary>
        public IConfiguration Configuration { get; } = configuration;

        /// <summary>
        /// The method <c>GetFetchSettingsSection</c> returns the specific <c>FetchSettings</c> configuration section 
        /// </summary>
        /// <returns>An object of type <see cref="IConfigurationSection"/></returns>
        public IConfigurationSection GetFetchSettingsSection() =>
            this.Configuration.GetSection("FetchSettings");

    }
}
