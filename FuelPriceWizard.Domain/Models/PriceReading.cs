namespace FuelPriceWizard.Domain.Models
{
    public class PriceReading : BaseModel
    {
        public decimal Value { get; set; }
        public DateTime FetchedAt { get; set; }

        public Currency Currency { get; set; } = new();
        public FuelType FuelType { get; set; } = new();
    }
}
