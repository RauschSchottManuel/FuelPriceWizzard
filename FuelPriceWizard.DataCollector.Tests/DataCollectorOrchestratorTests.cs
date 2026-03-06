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

        [Fact]
        public void CreateTasks_ReturnsEmptyList_WhenNoImplementationAssembliesConfigured()
        {
            // Arrange: Use a real empty IConfiguration so GetValue<bool> extension methods work correctly
            var emptyConfig = new ConfigurationBuilder().Build();

            _loggerFactoryMock
                .Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(new Mock<ILogger>().Object);

            var orchestrator = new DataCollectorOrchestrator(
                _loggerMock.Object,
                emptyConfig,
                _loggerFactoryMock.Object,
                _fuelTypeRepositoryMock.Object,
                _gasStationRepositoryMock.Object,
                _priceRepositoryMock.Object);

            // Act
            var tasks = orchestrator.CreateTasks();

            // Assert
            Assert.NotNull(tasks);
            Assert.Empty(tasks);
        }
    }
}