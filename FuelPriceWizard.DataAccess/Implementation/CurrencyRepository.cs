using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using CurrencyModel = FuelPriceWizard.Domain.Models.Currency;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class CurrencyRepository : BaseRepository<Currency, CurrencyModel>, ICurrencyRepository
    {
        public override string[] Includes => [];

        public CurrencyRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }


        public async Task<CurrencyModel> GetByAbbreviationAsync(string abbreviation)
        {
            var currency = await this.Context.Currencies.SingleOrDefaultAsync(c => c.Abbreviation == abbreviation);

            return this.Mapper.Map<CurrencyModel>(currency);
        }
    }
}
