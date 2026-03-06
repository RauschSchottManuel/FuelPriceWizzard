using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.DataCollector.Tests
{
    public class RepeatingTaskTests
    {
        private readonly Mock<ILogger> _loggerMock = new();

        private RepeatingTask<string> CreateTask(
            TimeSpan? interval = null,
            List<DayOfWeek>? excludedWeekdays = null)
        {
            return new RepeatingTask<string>(
                _loggerMock.Object,
                interval ?? TimeSpan.FromMilliseconds(100),
                "test-service",
                excludedWeekdays ?? []);
        }

        [Fact]
        public async Task Start_LogsWarning_WhenAlreadyRunning()
        {
            using var task = CreateTask();
            Func<ILogger, string, Task> noOp = (_, _) => Task.CompletedTask;

            await task.Start(noOp);
            await task.Start(noOp); // second call → should log warning

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task StopAsync_LogsWarning_WhenNotRunning()
        {
            using var task = CreateTask();

            await task.StopAsync(); // never started → should log warning

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task WaitForNextIncludedWeekday_ReturnsImmediately_WhenNoExcludedDays()
        {
            using var task = CreateTask(excludedWeekdays: []);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await task.WaitForNextIncludedWeekdayAsync().WaitAsync(cts.Token);
            // No exception → completed without blocking
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_WithoutException()
        {
            var task = CreateTask();

            var ex = Record.Exception(() =>
            {
                task.Dispose();
                task.Dispose();
            });

            Assert.Null(ex);
        }

        [Fact]
        public async Task StopAsync_AfterStart_CompletesSuccessfully()
        {
            var task = CreateTask();
            Func<ILogger, string, Task> noOp = (_, _) => Task.CompletedTask;

            await task.Start(noOp);

            var ex = await Record.ExceptionAsync(() => task.StopAsync());

            Assert.Null(ex);
        }
    }
}
