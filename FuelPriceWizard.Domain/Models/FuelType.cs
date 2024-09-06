namespace FuelPriceWizard.Domain.Models
{
    public class FuelType : BaseModel
    {
        public string DisplayValue { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
