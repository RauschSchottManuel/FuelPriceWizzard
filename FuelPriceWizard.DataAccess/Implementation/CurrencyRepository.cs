using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CurrencyModel = FuelPriceWizard.Domain.Models.Currency;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class CurrencyRepository : BaseRepository<Currency, CurrencyModel>, ICurrencyRepository
    {
        protected override Expression<Func<Currency, object>>[] Includes => [];

        public CurrencyRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<CurrencyModel> GetByAbbreviationAsync(string abbreviation, CancellationToken ct = default)
        {
            var currency = await this.Context.Currencies.SingleOrDefaultAsync(c => c.Abbreviation == abbreviation, ct);
            return this.Mapper.Map<CurrencyModel>(currency);
        }
    }
}
