using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class PriceReadingMappingProfile : Profile
    {
        public PriceReadingMappingProfile()
        {
            this.CreateMap<Entity.PriceReading, DomainModel.PriceReading>()
                .ReverseMap();
        }
    }
}
