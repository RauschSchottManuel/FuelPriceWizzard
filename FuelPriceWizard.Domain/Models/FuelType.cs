using System.ComponentModel.DataAnnotations;

namespace FuelPriceWizard.Domain.Models
{
    public class FuelType : BaseModel
    {
        [Required, MaxLength(100)]
        public string DisplayValue { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Abbreviation { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
