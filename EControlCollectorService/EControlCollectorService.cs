using FuelPriceWizard.BusinessLogic;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using EControlCollectorService.Model;
using System.Globalization;
using FuelType = FuelPriceWizard.Domain.Models.FuelType;

namespace EControlCollectorService
{
    public class EControlCollectorService : BaseFuelPriceSourceService, IFuelPriceSourceService
    {
        private readonly ILogger<EControlCollectorService> _logger;
        private readonly HttpClient _httpClient;
        public EControlCollectorService(IConfiguration config, HttpClient httpClient, ILogger<EControlCollectorService> logger) : base(config)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true)
        {
            var prices = new List<PriceReading>();

            foreach(var fuelType in Enum.GetValues(typeof(Enums.FuelType)))
            {
                prices.AddRange(await this.FetchPricesByLocationAndFuelTypeAsync(lat, lon, (Enums.FuelType) fuelType, includeClosed));
            }

            return prices;
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true)
        {
            this._logger.LogInformation("Starting to collect prices for location (latitude: {Latitude}, longitude: {Longitude}) and fuel type {FuelType} {IncludeClosed} ...", lat, lon, fuelType, includeClosed ? "including closed locations" : "excluding closed locations");

            var eControlFuelType = ConvertFuelTypeToEControlFuelType(fuelType);

            if (eControlFuelType is null)
            {
                this._logger.LogWarning("The specified fuel type ({FuelType}) is not supported by E-Control. Skipping this fetch operation.", fuelType);
                return [];
            }
            var queryParams = new Dictionary<string, string>()
            {
                { "latitude", lat.ToString(CultureInfo.GetCultureInfo("en-us")) },
                { "longitude", lon.ToString(CultureInfo.GetCultureInfo("en-us")) },
                { "fuelType", eControlFuelType },
                { "includeClosed", includeClosed.ToString() }
            };

            var requestUrl = string.Concat(this.Configuration.GetValue<string>("BaseFetchAddress"),
                "/search/gas-stations/by-address",
                $"?{string.Join('&', queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"))}");

            this._logger.LogInformation("Fetching prices from E-Control ...");
            var response = await this._httpClient.GetAsync(requestUrl);
            this._logger.LogInformation("Finished fetching prices from E-Control!");

            this._logger.LogInformation("Converting prices to price readings ...");

            var responseJson = await response.Content.ReadAsStringAsync();

            try
            {
                var gasStations = JsonSerializer.Deserialize<IEnumerable<EControlGasStation>>(responseJson);

                var requestedStation = gasStations?.FirstOrDefault();

                if(requestedStation == null)
                {
                    this._logger.LogError("No gas station found at the specified location! (lat: {Latitude}, lon: {Longitude})", lat, lon);
                    return [];
                }

                var prices = requestedStation.Prices.Select(p => new PriceReading
                {
                    Value = p.Amount,
                    FuelType = p.FuelType switch
                    {
                        "DIE" => new FuelType { DisplayValue = "Diesel" },
                        "SUP" => new FuelType { DisplayValue = "Super" },
                        _ => new FuelType { Abbreviation = p.FuelType },
                    }
                });

                this._logger.LogInformation("Completed collecting prices!");
                return prices.ToList();
            } catch(Exception ex)
            {
                this._logger.LogError(ex, "Something went wrong while parsing the response!");
                return [];
            }
        }

        private static string? ConvertFuelTypeToEControlFuelType(Enums.FuelType fuelType)
        {
            return fuelType switch
            {
                Enums.FuelType.Diesel => "DIE",
                Enums.FuelType.Super => "SUP",
                _ => null,
            };
        }
    }
}
