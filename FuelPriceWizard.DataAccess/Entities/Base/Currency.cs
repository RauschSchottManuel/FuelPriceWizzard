namespace FuelPriceWizard.DataAccess.Entities.Base
{
    public class Currency : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;

        public List<PriceReading> PriceReadings { get; set; } = new();
    }
}
