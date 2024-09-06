namespace FuelPriceWizard.DataAccess.Entities.Base
{
    public class GasStation : BaseEntity
    {
        public string Designation { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public int AddressId { get; set; }
        public Address? Address { get; set; }
        public List<FuelType> FuelTypes { get; set; } = new();

        public List<OpeningHours> OpeningHours { get; set; } = new();

        public List<PriceReading> PriceReadings { get; set; } = new();
    }
}
