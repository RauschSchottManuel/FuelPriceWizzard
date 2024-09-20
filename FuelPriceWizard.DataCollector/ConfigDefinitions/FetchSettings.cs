namespace FuelPriceWizard.DataCollector.ConfigDefinitions
{
    public class FetchSettings
    {
        public enum TimeUnit
        {
            Second,
            Minute,
            Hour,
        }

        public List<DayOfWeek> ExcludedWeekdays { get; set; } = new();
        public int IntervalValue { get; set; }
        public TimeUnit IntervalUnit { get; set; }
        public bool StartNextFullHour { get; set; } = false;
    }
}
