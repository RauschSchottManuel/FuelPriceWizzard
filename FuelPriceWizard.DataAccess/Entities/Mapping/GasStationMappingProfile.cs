using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class GasStationMappingProfile : Profile
    {
        public GasStationMappingProfile()
        {
            this.CreateMap<Entity.GasStation, DomainModel.GasStation>()
                .ReverseMap();
        }
    }
}
