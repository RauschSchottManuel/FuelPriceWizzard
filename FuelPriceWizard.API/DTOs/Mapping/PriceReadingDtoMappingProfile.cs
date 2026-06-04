using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class PriceReadingDtoMappingProfile : Profile
    {
        public PriceReadingDtoMappingProfile()
        {
            CreateMap<PriceReading, PriceReadingDto>().ReverseMap();
        }
    }
}
