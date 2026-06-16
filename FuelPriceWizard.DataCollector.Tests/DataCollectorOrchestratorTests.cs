using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.DataCollector.Tests
{
    public class DataCollectorOrchestratorTests
    {
        private readonly Mock<ILogger<DataCollectorOrchestrator>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IHostApplicationLifetime> _hostApplicationLifetimeMock;

        public DataCollectorOrchestratorTests()
        {
            _loggerMock = new Mock<ILogger<DataCollectorOrchestrator>>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>();
        }

        [Fact]
        public void DataCollectorOrchestrator_ShouldBeInstantiatedSuccessfully()
        {
            // Act
            var orchestrator = new DataCollectorOrchestrator(
                _loggerMock.Object,
                _configurationMock.Object,
                _loggerFactoryMock.Object,
                _serviceScopeFactoryMock.Object,
                _hostApplicationLifetimeMock.Object);

            // Assert
            Assert.NotNull(orchestrator);
            Assert.NotNull(orchestrator.Logger);
            Assert.NotNull(orchestrator.Configuration);
            Assert.NotNull(orchestrator.LoggerFactory);
        }
    }
}
