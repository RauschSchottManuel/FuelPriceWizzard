using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.BusinessLogic
{
    /// <summary>
    /// Core business logic service for the FuelPriceWizard application.
    /// </summary>
    public interface IFuelPriceWizardService
    {
        /// <summary>Gets all gas stations.</summary>
        Task<IEnumerable<GasStation>> GetAllGasStationsAsync();

        /// <summary>Gets a gas station by its identifier.</summary>
        Task<GasStation?> GetGasStationByIdAsync(int id);

        /// <summary>Gets the most recent price reading per fuel type for the given gas station.</summary>
        Task<IEnumerable<PriceReading>> GetLatestPricesForStationAsync(int stationId);

        /// <summary>Gets price history for a specific fuel type and gas station within a date range.</summary>
        Task<IEnumerable<PriceReading>> GetPriceHistoryAsync(int stationId, int fuelTypeId, DateTime from, DateTime to);

        /// <summary>Gets all active fuel types.</summary>
        Task<IEnumerable<FuelType>> GetAllFuelTypesAsync();

        /// <summary>Gets all currencies.</summary>
        Task<IEnumerable<Currency>> GetAllCurrenciesAsync();
    }
}
