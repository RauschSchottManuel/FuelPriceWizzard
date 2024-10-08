namespace FuelPriceWizard.DataCollector
{
    public interface IRepeatingTask<T>
    {
        Task Start(Func<T, Task> function);

        Task StopAsync();

        string GetGenericType();
    }
}
