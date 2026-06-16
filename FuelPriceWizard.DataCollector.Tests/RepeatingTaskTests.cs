using FuelPriceWizard.BusinessLogic;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPriceWizard.DataCollector.Tests
{
    public class RepeatingTaskTests
    {
        private static RepeatingTask<IFuelPriceSourceService> BuildTask(CancellationToken ct = default)
        {
            var loggerMock = new Mock<ILogger>();
            var serviceMock = new Mock<IFuelPriceSourceService>();
            return new RepeatingTask<IFuelPriceSourceService>(
                loggerMock.Object,
                TimeSpan.FromSeconds(30),
                serviceMock.Object,
                excludedWeekdays: [],
                startNextFullHour: false,
                cancellationToken: ct);
        }

        [Fact]
        public async Task Start_WhenAlreadyRunning_DoesNotStartTwice()
        {
            using var cts = new CancellationTokenSource();
            var task = BuildTask(cts.Token);
            var callCount = 0;

            await task.Start((_, _) =>
            {
                Interlocked.Increment(ref callCount);
                return Task.Delay(Timeout.Infinite, cts.Token);
            });

            await Task.Delay(50);

            // Second call should be ignored — running flag is already set.
            await task.Start((_, _) =>
            {
                Interlocked.Increment(ref callCount);
                return Task.CompletedTask;
            });

            await cts.CancelAsync();

            // Exactly one call: the first Start triggered the function once;
            // the second Start was a no-op.
            Assert.Equal(1, Volatile.Read(ref callCount));
        }

        [Fact]
        public async Task StopAsync_WhenNotRunning_DoesNotThrow()
        {
            var task = BuildTask();

            var ex = await Record.ExceptionAsync(() => task.StopAsync());

            Assert.Null(ex);
        }

        [Fact]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            var task = BuildTask();

            task.Dispose();
            var ex = Record.Exception(() => task.Dispose());

            Assert.Null(ex);
        }

        [Fact]
        public async Task WaitForNextIncludedWeekday_NoExcludedDays_ReturnsImmediately()
        {
            var task = BuildTask();

            var ex = await Record.ExceptionAsync(() => task.WaitForNextIncludedWeekdayAsync());

            Assert.Null(ex);
        }
    }
}
