namespace FuelPriceWizard.Domain.Models
{
    public class PriceReading : BaseModel
    {
        public decimal Value { get; set; }
        public DateTime FetchedAt { get; set; }

        public int CurrencyId { get; set; }
        public int FuelTypeId { get; set; }
        public int GasStationId { get; set; }

        public Currency? Currency { get; set; }
        public FuelType? FuelType { get; set; }
        public GasStation? GasStation { get; set; }
    }
}
