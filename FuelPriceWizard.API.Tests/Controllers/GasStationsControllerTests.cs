using AutoMapper;
using FuelPriceWizard.API.Controllers;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.API.DTOs.Mapping;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess.Exceptions;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.API.Tests.Controllers
{
    public class GasStationsControllerTests
    {
        private readonly Mock<ILogger<GasStationsController>> _loggerMock = new();
        private readonly Mock<IFuelPriceWizardService> _serviceMock = new();
        private readonly IMapper _mapper;

        public GasStationsControllerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AddressDtoMappingProfile>();
                cfg.AddProfile<FuelTypeDtoMappingProfile>();
                cfg.AddProfile<GasStationDtoMappingProfile>();
                cfg.AddProfile<OpeningHoursDtoMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        private GasStationsController CreateController() =>
            new(_loggerMock.Object, _mapper, _serviceMock.Object);

        [Fact]
        public async Task GetAll_ReturnsOk_WithPagedResult()
        {
            var stations = new List<GasStation> { new() { Id = 1, Designation = "Station A" } };
            _serviceMock.Setup(s => s.GetGasStationsPagedAsync(1, 20, default))
                .ReturnsAsync((stations, 1));

            var result = await CreateController().GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var paged = Assert.IsType<PagedResult<GasStationDto>>(ok.Value);
            Assert.Equal(1, paged.TotalCount);
            Assert.Single(paged.Items);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            var station = new GasStation { Id = 1, Designation = "Station B" };
            _serviceMock.Setup(s => s.GetGasStationByIdAsync(1, default)).ReturnsAsync(station);

            var result = await CreateController().GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GasStationDto>(ok.Value);
            Assert.Equal("Station B", dto.Designation);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetGasStationByIdAsync(99, default)).ReturnsAsync((GasStation?)null);

            var result = await CreateController().GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidDto_ReturnsCreated()
        {
            var dto = new GasStationDto { Designation = "New Station" };
            var inserted = new GasStation { Id = 5, Designation = "New Station" };
            _serviceMock.Setup(s => s.CreateGasStationAsync(It.IsAny<GasStation>(), default)).ReturnsAsync(inserted);

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/api/gasstations/5");

            var controller = CreateController();
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Url = urlHelperMock.Object;

            var result = await controller.Insert(dto);

            var created = Assert.IsType<CreatedResult>(result.Result);
            var returnedDto = Assert.IsType<GasStationDto>(created.Value);
            Assert.Equal("New Station", returnedDto.Designation);
        }

        [Fact]
        public async Task Update_ExistingStation_ReturnsOk()
        {
            var dto = new GasStationDto { Designation = "Updated" };
            var updated = new GasStation { Id = 1, Designation = "Updated" };
            _serviceMock.Setup(s => s.UpdateGasStationAsync(1, It.IsAny<GasStation>(), default)).ReturnsAsync(updated);

            var result = await CreateController().Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<GasStationDto>(ok.Value);
            Assert.Equal("Updated", returnedDto.Designation);
        }

        [Fact]
        public async Task Update_NonExistingStation_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.UpdateGasStationAsync(99, It.IsAny<GasStation>(), default))
                .ThrowsAsync(new NotFoundException("Not found"));

            var result = await CreateController().Update(99, new GasStationDto());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ExistingStation_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteGasStationAsync(1, default)).ReturnsAsync(true);

            var result = await CreateController().Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingStation_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteGasStationAsync(99, default)).ReturnsAsync(false);

            var result = await CreateController().Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
