namespace FuelPriceWizard.DataCollector
{
    public interface IDataCollectorOrchestrator
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task ReloadTasksAsync();
        void WatchForConfigurationChanges();
    }
}
