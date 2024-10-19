using FuelPriceWizard.BusinessLogic;

namespace FuelPriceWizard.DataCollector
{
    public interface IDataCollectorOrchestrator
    {
        public IEnumerable<RepeatingTask<IFuelPriceSourceService>> CreateTasks();
        public void StartTasks();
        public void StartTasks(IEnumerable<RepeatingTask<IFuelPriceSourceService>> tasks);
    }
}
