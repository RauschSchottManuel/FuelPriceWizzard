using FuelPriceWizard.DataAccess.Entities.Base;

namespace FuelPriceWizard.DataAccess.Entities
{
    public class PriceReading : BaseEntity
    {
        public decimal Value { get; set; }
        public DateTime FetchedAt { get; set; }

        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public int FuelTypeId { get; set; }
        public FuelType? FuelType { get; set; }

        public int GasStationId { get; set; }
        public GasStation? GasStation { get; set; }
    }
}
