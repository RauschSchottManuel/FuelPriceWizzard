using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.Domain.Models;
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

        /// <summary>
        /// The method <c>Setup</c> is used for additional setup of the collector like initializing variables.
        /// </summary>
        Task Setup();

        /// <summary>
        /// The method <c>FetchPricesByLocation</c> returns an <see cref="IEnumerable{T}"/> where T is of type <see cref="PriceReading"/>
        /// that contains all price readings for the specified gas station located at given latitude/longitude.
        /// </summary>
        /// <param name="lat">Defines the latitude of the gas station</param>
        /// <param name="lon">Defines the longitude of the gas station</param>
        /// <param name="includeClosed" >Specifies whether closed gas stations should be included. Default true</param>
        /// <returns><see cref="IEnumerable{T}"/> where T is of type <see cref="PriceReading"/>
        /// that contains all price readings for the specified gas station located at given latitude/longitude.</returns>
        Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true);

        /// <summary>
        /// The method <c>FetchPricesByLocation</c> returns an <see cref="IEnumerable{T}"/> where T is of type <see cref="PriceReading"/>
        /// that contains all price readings for the specified <see cref="Enums.FuelType">FuelType</see> and gas station located at given latitude/longitude.
        /// </summary>
        /// <param name="lat">Defines the latitude of the gas station</param>
        /// <param name="lon">Defines the longitude of the gas station</param>
        /// <param name="fuelType">Defines the specific fuel type to fetch prices for</param>
        /// <param name="includeClosed" >Specifies whether closed gas stations should be included. Default true</param>
        /// <returns><see cref="IEnumerable{T}"/> where T is of type <see cref="PriceReading"/>
        /// that contains all price readings for the specified <see cref="Enums.FuelType">FuelType</see> and gas station located at given latitude/longitude.
        /// </returns>
        Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true);
    }
}
