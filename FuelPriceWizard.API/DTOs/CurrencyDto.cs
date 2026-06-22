namespace FuelPriceWizard.API.DTOs
{
    public class CurrencyDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}
