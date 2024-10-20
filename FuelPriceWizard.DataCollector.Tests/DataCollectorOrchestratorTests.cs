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

        public DataCollectorOrchestratorTests()
        {
            _loggerMock = new Mock<ILogger<DataCollectorOrchestrator>>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _fuelTypeRepositoryMock = new Mock<IFuelTypeRepository>();
        }

        [Fact]
        public void DataCollectorOrchestrator_ShouldBeInstantiatedSuccessfully()
        {
            // Act
            var orchestrator = new DataCollectorOrchestrator(
                _loggerMock.Object,
                _configurationMock.Object,
                _loggerFactoryMock.Object,
                _fuelTypeRepositoryMock.Object);

            // Assert
            Assert.NotNull(orchestrator);
            Assert.NotNull(orchestrator.Logger);
            Assert.NotNull(orchestrator.Configuration);
            Assert.NotNull(orchestrator.LoggerFactory);
            Assert.NotNull(orchestrator.FuelTypeRepository);
        }

    }
}