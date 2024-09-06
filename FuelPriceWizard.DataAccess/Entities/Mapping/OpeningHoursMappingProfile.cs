using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class OpeningHoursMappingProfile : Profile
    {
        public OpeningHoursMappingProfile()
        {
            this.CreateMap<Entity.OpeningHours, DomainModel.OpeningHours>()
                .ReverseMap();
        }
    }
}
