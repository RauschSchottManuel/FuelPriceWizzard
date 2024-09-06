namespace FuelPriceWizard.DataAccess.Entities.Base
{
    public class OpeningHours : BaseEntity
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }

        public int GasStationId { get; set; }
        public GasStation GasStation { get; set; } = new();
    }
}
