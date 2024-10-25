using AutoMapper;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.DTOs.Mapping
{
    public class AddressDtoMappingProfile : Profile
    {
        public AddressDtoMappingProfile()
        {
            this.CreateMap<Address, AddressDto>()
                .ReverseMap();
        }
    }
}
