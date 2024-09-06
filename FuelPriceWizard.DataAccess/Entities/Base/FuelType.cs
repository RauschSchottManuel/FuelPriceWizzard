namespace FuelPriceWizard.DataAccess.Entities.Base
{
    public class FuelType : BaseEntity
    {
        public string DisplayValue { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<GasStation> GasStations { get; set; } = new();
        public List<PriceReading> PriceReadings { get; set; } = new();
    }
}
