using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.DataAccess
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Task<Currency> GetByAbbreviationAsync(string abbreviation);
    }
}
