using FuelPriceWizard.BusinessLogic.Modules.Enums;
using Microsoft.Extensions.Configuration;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// <c>IFuelPriceSourceService</c> defines the interface implemented by collector service classes
    /// </summary>
    public interface IFuelPriceSourceService
    {
        /// <summary>
        /// The method <c>GetFetchSettingsSection</c> returns the specific <c>FetchSettings</c> configuration section 
        /// </summary>
        /// <returns>An object of type <see cref="IConfigurationSection"/></returns>
        IConfigurationSection GetFetchSettingsSection();
        object FetchPricesByLocation(int lat, int lon);
        object FetchPricesByLocationAndFuelType(int lat, int lon, FuelType fuelType, int maxReturnResults = 1);
        
        //TODO: define interface for fetching a data source for fuel prices
    }
}
