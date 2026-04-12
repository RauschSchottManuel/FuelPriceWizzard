using System.ComponentModel.DataAnnotations;

namespace FuelPriceWizard.API.DTOs
{
    public class CurrencyDto : BaseDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Abbreviation { get; set; } = string.Empty;

        [Required]
        [MaxLength(5)]
        public string Symbol { get; set; } = string.Empty;
    }
}
