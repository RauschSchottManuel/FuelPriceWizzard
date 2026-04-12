namespace FuelPriceWizard.API.DTOs
{
    public class PriceReadingDto : BaseDto
    {
        public decimal Value { get; set; }
        public DateTime FetchedAt { get; set; }
        public int GasStationId { get; set; }
        public int FuelTypeId { get; set; }
        public int CurrencyId { get; set; }
        public FuelTypeDto? FuelType { get; set; }
        public CurrencyDto? Currency { get; set; }
    }
}
