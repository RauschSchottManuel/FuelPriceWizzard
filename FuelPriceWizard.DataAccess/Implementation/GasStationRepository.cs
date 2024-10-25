using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using GasStationModel = FuelPriceWizard.Domain.Models.GasStation;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class GasStationRepository : BaseRepository<GasStation, GasStationModel>, IGasStationRepository
    {
        public override string[] Includes => [ nameof(GasStation.FuelTypes) ];

        public GasStationRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

    }
}
