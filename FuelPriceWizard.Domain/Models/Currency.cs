namespace FuelPriceWizard.Domain.Models
{
    public class Currency : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}
