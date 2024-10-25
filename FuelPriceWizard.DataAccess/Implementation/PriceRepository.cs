using AutoMapper;
using FuelPriceWizard.DataAccess.Entities;
using PriceModel = FuelPriceWizard.Domain.Models.PriceReading;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class PriceRepository : BaseRepository<PriceReading, PriceModel>, IPriceRepository
    {
        public override string[] Includes => [ nameof(PriceReading.Currency) ];

        public PriceRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
}
