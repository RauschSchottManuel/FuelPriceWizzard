using System.Text.Json.Serialization;

namespace EControlCollectorService.Model
{
    public class EControlPriceReading
    {
        [JsonPropertyName("fuelType")]
        public string FuelType { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }
}
