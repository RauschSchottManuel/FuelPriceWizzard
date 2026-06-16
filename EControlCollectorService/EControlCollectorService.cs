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

        public override Dictionary<string, Enums.FuelType> FuelTypeMapping => new()
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
            var tasks = Enum.GetValues<Enums.FuelType>()
                .Select(ft => FetchPricesByLocationAndFuelTypeAsync(lat, lon, ft, includeClosed));

            var results = await Task.WhenAll(tasks);
            return results.SelectMany(r => r);
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true)
        {
            this.Logger.LogInformation("Starting to collect prices for location (latitude: {Latitude}, longitude: {Longitude}) and fuel type {FuelType} {IncludeClosed} ...",
                lat, lon, fuelType, includeClosed ? "including closed locations" : "excluding closed locations");

            var eControlFuelType = MapFromFuelType(fuelType);

            if (string.IsNullOrEmpty(eControlFuelType))
            {
                this.Logger.LogWarning("The specified fuel type ({FuelType}) is not supported by E-Control. Skipping this fetch operation.", fuelType);
                return [];
            }

            var baseFetchAddress = this.Configuration.GetValue<string>("BaseFetchAddress")
                ?? throw new InvalidOperationException("BaseFetchAddress is not configured.");

            var queryParams = new Dictionary<string, string>
            {
                { "latitude", lat.ToString(CultureInfo.InvariantCulture) },
                { "longitude", lon.ToString(CultureInfo.InvariantCulture) },
                { "fuelType", eControlFuelType },
                { "includeClosed", includeClosed.ToString() }
            };

            var requestUrl = string.Concat(
                baseFetchAddress,
                "/search/gas-stations/by-address",
                "?",
                string.Join('&', queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}")));

            this.Logger.LogInformation("Fetching prices from E-Control: {RequestUrl}", requestUrl);

            HttpResponseMessage response;
            try
            {
                response = await this._httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                this.Logger.LogError(ex, "HTTP request to E-Control failed (status: {StatusCode}).", ex.StatusCode);
                return [];
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            try
            {
                var gasStations = JsonSerializer.Deserialize<IEnumerable<EControlGasStation>>(responseJson);

                if (gasStations is null)
                {
                    this.Logger.LogError("Failed to deserialize E-Control response for location (lat: {Latitude}, lon: {Longitude}).", lat, lon);
                    return [];
                }

                var stationList = gasStations.ToList();
                this.Logger.LogDebug("E-Control returned {Count} station(s) for fuel type {FuelType}.", stationList.Count, fuelType);

                // Select the station geometrically closest to the queried coordinates.
                // E-Control does not guarantee result order, so we compute squared-Euclidean
                // distance (sufficient for proximity comparison at this scale).
                var nearestStation = stationList
                    .Where(s => s.Location is not null)
                    .MinBy(s => Math.Pow(s.Location!.Latitude - (double)lat, 2)
                              + Math.Pow(s.Location!.Longitude - (double)lon, 2))
                    ?? stationList.FirstOrDefault();

                if (nearestStation is null)
                {
                    this.Logger.LogWarning("E-Control returned no stations for location (lat: {Latitude}, lon: {Longitude}).", lat, lon);
                    return [];
                }

                var distanceSq = nearestStation.Location is not null
                    ? Math.Pow(nearestStation.Location.Latitude - (double)lat, 2)
                      + Math.Pow(nearestStation.Location.Longitude - (double)lon, 2)
                    : double.NaN;

                var loc = nearestStation.Location;
                this.Logger.LogInformation(
                    "Matched E-Control station '{StationName}' (ID {StationId}), {Address}, {PostalCode} {City} at ({StationLat}, {StationLon}), distance² = {DistanceSq:F6} from query ({Lat}, {Lon}).",
                    nearestStation.Name, nearestStation.Id,
                    loc?.Address, loc?.PostalCode, loc?.City,
                    loc?.Latitude, loc?.Longitude,
                    distanceSq, lat, lon);

                var currencyId = (await GetCurrencyObjectAsync())?.Id ?? 0;
                var prices = new List<PriceReading>();

                foreach (var p in nearestStation.Prices)
                {
                    var mappedFuelType = await MapToFuelTypeAsync(p.FuelType);
                    if (mappedFuelType is null)
                    {
                        this.Logger.LogDebug(
                            "Skipping unsupported fuel type '{EControlFuelType}' from station '{StationName}' (ID {StationId}), {Address}, {PostalCode} {City}.",
                            p.FuelType, nearestStation.Name, nearestStation.Id,
                            loc?.Address, loc?.PostalCode, loc?.City);
                        continue;
                    }

                    prices.Add(new PriceReading
                    {
                        Value = p.Amount,
                        FuelTypeId = mappedFuelType.Id,
                        CurrencyId = currencyId,
                        FetchedAt = DateTime.UtcNow,
                    });

                    this.Logger.LogDebug(
                        "  {FuelType} ({EControlCode}): {Amount} (FuelTypeId={FuelTypeId}, CurrencyId={CurrencyId})",
                        mappedFuelType.Abbreviation, p.FuelType, p.Amount, mappedFuelType.Id, currencyId);
                }

                this.Logger.LogInformation(
                    "Collected {Count} price reading(s) for fuel type {FuelType} from station '{StationName}' (ID {StationId}), {Address}, {PostalCode} {City}.",
                    prices.Count, fuelType, nearestStation.Name, nearestStation.Id,
                    loc?.Address, loc?.PostalCode, loc?.City);

                return prices;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Something went wrong while parsing the E-Control response!");
                return [];
            }
        }
    }
}
