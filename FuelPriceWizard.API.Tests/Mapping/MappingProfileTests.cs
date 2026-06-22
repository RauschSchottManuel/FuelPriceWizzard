using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.API.DTOs.Mapping;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.API.Tests.Mapping
{
    public class MappingProfileTests
    {
        private static IMapper BuildMapper() =>
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AddressDtoMappingProfile>();
                cfg.AddProfile<CurrencyDtoMappingProfile>();
                cfg.AddProfile<FuelTypeDtoMappingProfile>();
                cfg.AddProfile<GasStationDtoMappingProfile>();
                cfg.AddProfile<OpeningHoursDtoMappingProfile>();
                cfg.AddProfile<PriceReadingDtoMappingProfile>();
            }).CreateMapper();

        [Fact]
        public void AllProfiles_AreValid()
        {
            BuildMapper().ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void GasStation_MapsToDto_Correctly()
        {
            var mapper = BuildMapper();
            var station = new GasStation
            {
                Id = 42,
                Designation = "Test Station",
                IsActive = true,
                Address = new Address { Street = "Main St", Zip = "1234", City = "Vienna", Country = "Austria" }
            };

            var dto = mapper.Map<GasStationDto>(station);

            Assert.Equal(42, dto.Id);
            Assert.Equal("Test Station", dto.Designation);
            Assert.Equal("Main St", dto.Address.Street);
        }

        [Fact]
        public void GasStationDto_MapsToModel_SetsIsActiveTrue()
        {
            var mapper = BuildMapper();
            var dto = new GasStationDto { Designation = "Station X" };

            var model = mapper.Map<GasStation>(dto);

            Assert.True(model.IsActive);
        }
    }
}
