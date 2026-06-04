using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using System.Linq.Expressions;
using GasStationModel = FuelPriceWizard.Domain.Models.GasStation;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class GasStationRepository : BaseRepository<GasStation, GasStationModel>, IGasStationRepository
    {
        protected override Expression<Func<GasStation, object>>[] Includes =>
            [g => g.FuelTypes];

        public GasStationRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
