using System.ComponentModel.DataAnnotations;

namespace FuelPriceWizard.Domain.Models
{
    public class Currency : BaseModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Abbreviation { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;
    }
}
