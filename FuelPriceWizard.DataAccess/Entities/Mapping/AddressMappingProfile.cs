using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            this.CreateMap<Entity.Address, DomainModel.Address>()
                .ReverseMap();
        }
    }
}
