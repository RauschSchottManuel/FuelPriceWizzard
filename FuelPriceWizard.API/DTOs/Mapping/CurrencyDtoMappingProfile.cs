using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class CurrencyDtoMappingProfile : Profile
    {
        public CurrencyDtoMappingProfile()
        {
            CreateMap<Currency, CurrencyDto>().ReverseMap();
        }
    }
}
