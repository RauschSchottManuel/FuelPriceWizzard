using AutoMapper;
using FuelPriceWizard.API.Controllers;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.API.Tests
{
    public class GasStationsControllerTests
    {
        private readonly Mock<ILogger<GasStationsController>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IGasStationRepository> _repoMock = new();

        private GasStationsController CreateController()
        {
            var controller = new GasStationsController(
                _loggerMock.Object,
                _mapperMock.Object,
                _repoMock.Object);

            // Provide a minimal HttpContext so URL helper and ModelState work
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("/api/gasstations/1");
            controller.Url = urlHelperMock.Object;

            return controller;
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithMappedDtos()
        {
            var stations = new List<GasStation> { new() { Id = 1, Designation = "Shell" } };
            var dtos = new List<GasStationDto> { new() { Id = 1, Designation = "Shell" } };

            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<string[]>())).ReturnsAsync(stations);
            _mapperMock.Setup(m => m.Map<IEnumerable<GasStationDto>>(stations)).Returns(dtos);

            var controller = CreateController();
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dtos, ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenGasStationExists()
        {
            var station = new GasStation { Id = 1, Designation = "BP" };

            _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<string[]>())).ReturnsAsync(station);

            var controller = CreateController();
            var result = await controller.GetById(1);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenGasStationDoesNotExist()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<string[]>())).ReturnsAsync((GasStation?)null);

            var controller = CreateController();
            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task InsertNew_ReturnsCreated_WhenValidDtoProvided()
        {
            var dto = new GasStationDto { Designation = "OMV" };
            var domainStation = new GasStation { Id = 5, Designation = "OMV" };

            _mapperMock.Setup(m => m.Map<GasStation>(dto)).Returns(domainStation);
            _repoMock.Setup(r => r.InsertAsync(domainStation)).ReturnsAsync(domainStation);

            var controller = CreateController();
            var result = await controller.InsertNew(dto);

            Assert.IsType<CreatedResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenValidDtoProvided()
        {
            var dto = new GasStationDto { Id = 3, Designation = "Jet" };
            var domainStation = new GasStation { Id = 3, Designation = "Jet" };

            _mapperMock.Setup(m => m.Map<GasStation>(dto)).Returns(domainStation);
            _repoMock.Setup(r => r.UpdateAsync(3, domainStation)).ReturnsAsync(domainStation);

            var controller = CreateController();
            var result = await controller.Update(3, dto);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            _repoMock.Setup(r => r.DeleteByIdAsync(7)).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.Delete(7);

            Assert.IsType<NoContentResult>(result.Result);
        }
    }
}
