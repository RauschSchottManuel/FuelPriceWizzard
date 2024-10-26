using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class GasStationDtoMappingProfile : Profile
    {
        public GasStationDtoMappingProfile()
        {
            CreateMap<GasStation, GasStationDto>()
                .ReverseMap()
                .ForMember(m => m.IsActive, o => o.MapFrom(m => true));
        }
    }
}
