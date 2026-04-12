using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.DataAccess
{
    public interface IPriceRepository : IRepository<PriceReading>
    {
        /// <summary>Returns the most recent price reading per fuel type for the given gas station.</summary>
        Task<IEnumerable<PriceReading>> GetLatestByStationAsync(int stationId);

        /// <summary>Returns all price readings for a specific fuel type and gas station within the given date range, ordered by time ascending.</summary>
        Task<IEnumerable<PriceReading>> GetHistoryAsync(int stationId, int fuelTypeId, DateTime from, DateTime to);
    }
}
