using System.Text.Json.Serialization;

namespace EControlCollectorService.Model
{
    public class EControlGasStation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("prices")]
        public IEnumerable<EControlPriceReading> Prices { get; set; } = [];
    }
}
