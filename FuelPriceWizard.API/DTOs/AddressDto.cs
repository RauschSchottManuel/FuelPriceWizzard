using System.ComponentModel.DataAnnotations;

namespace FuelPriceWizard.API.DTOs
{
    public class AddressDto : BaseDto
    {
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Zip { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Range(-90.0, 90.0)]
        public double? Lat { get; set; }

        [Range(-180.0, 180.0)]
        public double? Long { get; set; }
    }
}
