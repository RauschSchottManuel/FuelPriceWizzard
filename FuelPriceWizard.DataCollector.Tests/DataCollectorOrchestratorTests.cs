using FuelPriceWizard.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.DataCollector.Tests
{
    public class DataCollectorOrchestratorTests
    {
        private readonly Mock<ILogger<DataCollectorOrchestrator>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<IFuelTypeRepository> _fuelTypeRepositoryMock;
        private readonly Mock<IGasStationRepository> _gasStationRepositoryMock;
        private readonly Mock<IPriceRepository> _priceRepositoryMock;

        public DataCollectorOrchestratorTests()
        {
            _loggerMock = new Mock<ILogger<DataCollectorOrchestrator>>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _fuelTypeRepositoryMock = new Mock<IFuelTypeRepository>();
            _gasStationRepositoryMock = new Mock<IGasStationRepository>();
            _priceRepositoryMock = new Mock<IPriceRepository>();
        }

        [Fact]
        public void DataCollectorOrchestrator_ShouldBeInstantiatedSuccessfully()
        {
            // Act
            var orchestrator = new DataCollectorOrchestrator(
                _loggerMock.Object,
                _configurationMock.Object,
                _loggerFactoryMock.Object,
                _fuelTypeRepositoryMock.Object,
                _gasStationRepositoryMock.Object,
                _priceRepositoryMock.Object);

            // Assert
            Assert.NotNull(orchestrator);
            Assert.NotNull(orchestrator.Logger);
            Assert.NotNull(orchestrator.Configuration);
            Assert.NotNull(orchestrator.LoggerFactory);
            Assert.NotNull(orchestrator.FuelTypeRepository);
        }
    }
}