using System.ComponentModel.DataAnnotations;

namespace FuelPriceWizard.Domain.Models
{
    public class PriceReading : BaseModel
    {
        [Range(0.001, double.MaxValue)]
        public decimal Value { get; set; }

        public DateTime FetchedAt { get; set; }

        [Range(1, int.MaxValue)]
        public int CurrencyId { get; set; }

        [Range(1, int.MaxValue)]
        public int FuelTypeId { get; set; }

        [Range(1, int.MaxValue)]
        public int GasStationId { get; set; }

        public Currency? Currency { get; set; }
        public FuelType? FuelType { get; set; }
        public GasStation? GasStation { get; set; }
    }
}
