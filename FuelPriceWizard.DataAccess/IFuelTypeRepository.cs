using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.DataAccess
{
    public interface IFuelTypeRepository : IRepository<FuelType>
    {
        Task<FuelType> GetByDisplayValueAsync(string displayValue, CancellationToken ct = default);
    }
}
