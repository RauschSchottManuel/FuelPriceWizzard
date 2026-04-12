using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class PriceReadingDtoMappingProfile : Profile
    {
        public PriceReadingDtoMappingProfile()
        {
            CreateMap<PriceReading, PriceReadingDto>()
                .ForMember(d => d.FuelType, o => o.AllowNull())
                .ForMember(d => d.Currency, o => o.AllowNull())
                .ReverseMap();
        }
    }
}
