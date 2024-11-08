namespace FuelPriceWizard.API.DTOs
{
    public class GasStationDto : BaseDto
    {
        public string Designation { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new();
        public List<FuelTypeDto> FuelTypes { get; set; } = [];
        public List<OpeningHoursDto> OpeningHours { get; set; } = [];
    }
}
