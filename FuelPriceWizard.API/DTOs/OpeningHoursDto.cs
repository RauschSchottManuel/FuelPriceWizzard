namespace FuelPriceWizard.API.DTOs
{
    public class OpeningHoursDto : BaseDto
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
    }
}
