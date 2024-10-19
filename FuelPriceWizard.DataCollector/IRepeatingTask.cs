using Serilog;

namespace FuelPriceWizard.DataCollector
{
    public interface IRepeatingTask<T>
    {
        Task Start(Func<ILogger, T, Task> function);

        Task StopAsync();

        string GetGenericType();
    }
}
