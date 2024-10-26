using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class FuelTypeDtoMappingProfile : Profile
    {
        public FuelTypeDtoMappingProfile()
        {
            this.CreateMap<FuelType, FuelTypeDto>()
                .ReverseMap()
                .ForMember(m => m.IsActive, o => o.MapFrom(m => true));
        }
    }
}
