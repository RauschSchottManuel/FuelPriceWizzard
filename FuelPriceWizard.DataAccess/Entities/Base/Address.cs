namespace FuelPriceWizard.DataAccess.Entities.Base
{
    public class Address : BaseEntity
    {
        public string Street { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Long { get; set; }

        public int GasStationId { get; set; }
        public GasStation? GasStation { get; set; }

    }
}
