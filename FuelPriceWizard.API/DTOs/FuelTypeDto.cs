namespace FuelPriceWizard.API.DTOs
{
    public class FuelTypeDto : BaseDto
    {
        public string DisplayValue { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
    }
}
