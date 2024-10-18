using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.DataAccess
{
    public interface IFuelTypeRepository : IRepository<FuelType>
    {
        public Task<FuelType> GetByDisplayValueAsync(string displayValue);
    }
}
