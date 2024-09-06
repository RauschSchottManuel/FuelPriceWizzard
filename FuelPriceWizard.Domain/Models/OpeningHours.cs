namespace FuelPriceWizard.Domain.Models
{
    public class OpeningHours : BaseModel
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
    }
}
