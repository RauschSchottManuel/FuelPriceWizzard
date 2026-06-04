using AutoMapper;
using FuelPriceWizard.DataAccess.Entities;
using System.Linq.Expressions;
using PriceModel = FuelPriceWizard.Domain.Models.PriceReading;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class PriceRepository : BaseRepository<PriceReading, PriceModel>, IPriceRepository
    {
        protected override Expression<Func<PriceReading, object>>[] Includes =>
            [p => p.Currency!];

        public PriceRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
