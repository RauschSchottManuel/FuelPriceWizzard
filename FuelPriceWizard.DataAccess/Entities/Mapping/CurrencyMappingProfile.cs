using AutoMapper;
using DomainModel = FuelPriceWizard.Domain.Models;
using Entity = FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities.Mapping
{
    public class CurrencyMappingProfile : Profile
    {
        public CurrencyMappingProfile()
        {
            this.CreateMap<Entity.Currency, DomainModel.Currency>()
                .ReverseMap();
        }
    }
}
