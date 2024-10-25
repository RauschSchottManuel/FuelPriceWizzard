using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class OpeningHoursDtoMappingProfile : Profile
    {
        public OpeningHoursDtoMappingProfile()
        {
            this.CreateMap<OpeningHours, OpeningHoursDto>()
                .ReverseMap();
        }
    }
}
