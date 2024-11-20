using EControlCollectorService.Model;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace EControlCollectorService
{
    public class EControlCollectorService : BaseFuelPriceSourceService<EControlCollectorService>, IFuelPriceSourceService
    {
        private readonly HttpClient _httpClient;

        public override Dictionary<string, Enums.FuelType> FuelTypeMapping=> new()
        {
            { "DIE", Enums.FuelType.Diesel },
            { "SUP", Enums.FuelType.Super },
        };

        public override Enums.Currency Currency => Enums.Currency.EUR;

        public EControlCollectorService(IConfiguration config,
            HttpClient httpClient,
            ILogger<EControlCollectorService> logger,
            IFuelTypeRepository fuelTypeRepository,
            ICurrencyRepository currencyRepository)
            : base(config, logger, fuelTypeRepository, currencyRepository)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true)
        {
            var prices = new List<PriceReading>();

            foreach (var fuelType in Enum.GetValues(typeof(Enums.FuelType)))
            {
                prices.AddRange(await this.FetchPricesByLocationAndFuelTypeAsync(lat, lon, (Enums.FuelType)fuelType, includeClosed));
            }

            return prices;
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true)
        {
            this.Logger.LogInformation("Starting to collect prices for location (latitude: {Latitude}, longitude: {Longitude}) and fuel type {FuelType} {IncludeClosed} ...", lat, lon, fuelType, includeClosed ? "including closed locations" : "excluding closed locations");

            var eControlFuelType = MapFromFuelType(fuelType);

            if (eControlFuelType is null)
            {
                this.Logger.LogWarning("The specified fuel type ({FuelType}) is not supported by E-Control. Skipping this fetch operation.", fuelType);
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

            this.Logger.LogInformation("Fetching prices from E-Control ...");
            var response = await this._httpClient.GetAsync(requestUrl);

            var responseJson = await response.Content.ReadAsStringAsync();

            try
            {
                var gasStations = JsonSerializer.Deserialize<IEnumerable<EControlGasStation>>(responseJson);

                var requestedStation = gasStations?.FirstOrDefault();

                if (requestedStation == null)
                {
                    this.Logger.LogError("No gas station found at the specified location! (lat: {Latitude}, lon: {Longitude})", lat, lon);
                    return [];
                }

                var prices = requestedStation.Prices.Select(p => new PriceReading
                {
                    Value = p.Amount,
                    FuelType = this.FuelTypeObjectMapping.GetValueOrDefault(Enum.Parse<Enums.FuelType>(p.FuelType)) ?? new(),
                    Currency = this.CurrencyObject,
                });

                this.Logger.LogInformation("Completed collecting prices!");
                return prices.ToList();
                //return await Task.WhenAll(prices.ToList());
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Something went wrong while parsing the response!");
                return [];
            }
        }
    }
}
