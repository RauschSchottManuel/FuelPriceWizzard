using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using GasStationModel = FuelPriceWizard.Domain.Models.GasStation;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class StationRepository : BaseRepository<GasStation, GasStationModel>, IStationRepository
    {
        public StationRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
