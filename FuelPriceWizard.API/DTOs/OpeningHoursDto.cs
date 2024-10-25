namespace FuelPriceWizard.API.DTOs
{
    public class OpeningHoursDto
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
    }
}
