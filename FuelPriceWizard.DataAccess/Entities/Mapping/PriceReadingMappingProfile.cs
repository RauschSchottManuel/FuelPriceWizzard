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
                .ForMember(p => p.Currency, options => options.AllowNull())
                .ForMember(p => p.FuelType, options => options.AllowNull())
                .ForMember(p => p.GasStation, options => options.AllowNull())
                .ReverseMap();
        }
    }
}
