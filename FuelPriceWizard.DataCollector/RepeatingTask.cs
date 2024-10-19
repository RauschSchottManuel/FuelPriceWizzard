using Microsoft.Extensions.Logging;

namespace FuelPriceWizard.DataCollector
{
    public class RepeatingTask<T>(ILogger logger, TimeSpan interval, T service,
        List<DayOfWeek> excludedWeekdays, bool startNextFullHour = false, CancellationToken cancellationToken = default) : IDisposable
    {
        private readonly T _service = service;
        private readonly List<DayOfWeek> _excludedWeekdays = excludedWeekdays;
        private readonly bool _startNextFullHour = startNextFullHour;
        private readonly PeriodicTimer _timer = new(interval);
        private Task? _periodicTask;

        private bool _isRunning = false, _disposed = false;

        public async Task Start(Func<ILogger, T, Task> function)
        {
            if (_isRunning)
            {
                logger.LogWarning("Attempt to start a task that is already running.");
                return;
            }

            _isRunning = true;

            try
            {
                logger.LogInformation("Starting collector service ...");
                if(_startNextFullHour)
                {
                    await WaitForNextFullHourAsync();
                }
                _periodicTask = this.ExecutePeriodically(function);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong while starting the task for {TaskName}!", nameof(T));
            }
        }

        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                logger.LogWarning("Attempt to stop a task that is not running.");
                return;
            }

            logger.LogInformation("Stopping collector service ...");


            _isRunning = false;

            _timer.Dispose(); // Stop the timer
            logger.LogInformation("The timer for {TaskType} is disposed.", nameof(T));

            if (_periodicTask is not null)
            {
                try
                {
                    await _periodicTask;
                }
                catch (OperationCanceledException ex)
                {
                    logger.LogWarning(ex, "The periodic task for {TaskType} was cancelled.", nameof(T));
                }
            }

            logger.LogInformation("The periodic task for {TaskType} has been stopped.", nameof(T));
        }

        public string GetGenericType() =>
            _service!.GetType().GetGenericArguments()[0].ToString();

        public async Task ExecutePeriodically(Func<ILogger, T, Task> function)
        {
            try
            {
                do
                {
                    await WaitForNextIncludedWeekdayAsync();

                    try
                    {
                        await function(logger, _service);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error executing function for {TaskName}.", nameof(T));
                    }

                } while (await _timer.WaitForNextTickAsync(cancellationToken));
            }
            catch (OperationCanceledException ex)
            {
                logger.LogWarning(ex, "The task was canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in periodic execution for {TaskName}.", nameof(T));
            }
        }

        private async Task WaitForNextFullHourAsync()
        {
            DateTime now = DateTime.UtcNow;

            // Move to the next full hour
            DateTime nextFullHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);

            // Log the next full hour
            logger.LogInformation("Task is configured to start at the next full hour (UTC). First execution on {NextFullHour:dd.MM.yyyy HH:mm}!", nextFullHour);

            // Calculate the delay duration
            TimeSpan delay = nextFullHour - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(delay);
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogWarning(ex, "The delay until the next full hour was canceled.");
                    throw;
                }
            }
        }

        public async Task WaitForNextIncludedWeekdayAsync()
        {
            while (_excludedWeekdays.Contains(DateTime.UtcNow.DayOfWeek))
            {
                // Calculate the next execution date (start of next day)
                var nextExecutionDate = DateTime.UtcNow.Date.AddDays(1);
                var delayDuration = nextExecutionDate - DateTime.UtcNow;

                logger.LogInformation("Fetch settings are configured to not run on the following days: {ExcludedDays}. "
                    + "Next execution attempt will be on {NextTryDate:dd.MM.yyyy HH:mm}",
                    _excludedWeekdays, nextExecutionDate);

                try
                {
                    // Delay until the next execution time, or until canceled
                    await Task.Delay(delayDuration, cancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogInformation(ex, "Task was canceled before the next scheduled execution.");
                    throw;
                }
            }
        }

        // The public Dispose method that implements IDisposable
        public void Dispose()
        {
            Dispose(true);

            // Suppress finalization since resources have already been disposed
            GC.SuppressFinalize(this);
        }

        // Protected method that disposes of resources
        protected virtual void Dispose(bool disposing)
        {
            // Check if the object has already been disposed
            if (_disposed)
                return;

            if (disposing)
            {
                // Free managed resources here (e.g., managed disposable objects)
                _timer?.Dispose();
                logger.LogInformation("Timer disposed for {TaskType}.", nameof(T));
            }

            _disposed = true;
        }
    }
}
