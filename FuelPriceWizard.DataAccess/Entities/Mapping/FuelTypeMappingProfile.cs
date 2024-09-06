using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class FuelTypeMappingProfile : Profile
    {
        public FuelTypeMappingProfile()
        {
            this.CreateMap<Entity.FuelType, DomainModel.FuelType>()
                .ReverseMap();
        }
    }
}
