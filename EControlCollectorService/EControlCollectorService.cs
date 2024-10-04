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
    public class EControlCollectorService : IFuelPriceSourceService
    {
        private readonly ILogger<EControlCollectorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public EControlCollectorService(IConfiguration config, HttpClient httpClient, ILogger<EControlCollectorService> logger)
        {
            _configuration = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        public IConfigurationSection GetFetchSettingsSection() =>
            _configuration.GetSection("FetchSettings");

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
            var queryParams = new Dictionary<string, string>()
            {
                { "latitude", lat.ToString(CultureInfo.GetCultureInfo("en-us")) },
                { "longitude", lon.ToString(CultureInfo.GetCultureInfo("en-us")) },
                { "fuelType", ConvertFuelTypeToEControlFuelType(fuelType) },
                { "includeClosed", includeClosed.ToString() }
            };

            var requestUrl = string.Concat(_configuration.GetValue<string>("BaseFetchAddress"),
                "/search/gas-stations/by-address",
                $"?{string.Join('&', queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"))}");

            this._logger.LogInformation("Retrieving prices from {0} ..", requestUrl);
            var response = await this._httpClient.GetAsync(requestUrl);
            this._logger.LogInformation("Finished retrieving prices!");

            this._logger.LogInformation("Converting prices to price readings ...");

            var responseJson = await response.Content.ReadAsStringAsync();

            try
            {
                var gasStations = JsonSerializer.Deserialize<IEnumerable<EControlGasStation>>(responseJson);

                var requestedStation = gasStations?.FirstOrDefault();

                if(requestedStation == null)
                {
                    this._logger.LogError("No gas station found at the specified coordinates! (lat: {0}, lon: {1})", lat, lon);
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

                this._logger.LogInformation("Finished fetching prices!");
                return prices.ToList();
            } catch(Exception ex)
            {
                this._logger.LogError("Something went wrong while parsing the response: {0} {1} {2}", ex.Message, ex.StackTrace, ex.InnerException);
                return [];
            }
        }

        private static string ConvertFuelTypeToEControlFuelType(Enums.FuelType fuelType)
        {
            return fuelType switch
            {
                Enums.FuelType.Diesel => "DIE",
                Enums.FuelType.PremiumDiesel => "DIE",
                Enums.FuelType.Super => "SUP",
                Enums.FuelType.SuperPlus => "SUP",
                _ => "DIE",
            };
        }
    }
}
