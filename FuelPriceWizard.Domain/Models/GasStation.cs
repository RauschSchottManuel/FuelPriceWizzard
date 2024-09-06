namespace FuelPriceWizard.Domain.Models
{
    public class GasStation : BaseModel
    {
        public string Designation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Address? Address { get; set; }
        public List<FuelType> FuelTypes { get; set; } = new();
        public List<OpeningHours> OpeningHours { get; set; } = new();
    }
}
