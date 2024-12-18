﻿using System.Text.Json.Serialization;

namespace FuelPriceWizard.API.DTOs
{
    public class AddressDto : BaseDto
    {
        public string Street { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double? Lat { get; set; }
        public double? Long { get; set; }
    }
}
