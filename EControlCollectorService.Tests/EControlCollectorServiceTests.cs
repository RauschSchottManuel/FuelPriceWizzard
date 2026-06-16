using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using EControlCollectorService.Model;

namespace EControlCollectorService.Tests
{
    public class EControlCollectorServiceTests
    {
        private static readonly FuelType DieselType = new() { Id = 1, DisplayValue = "Diesel", Abbreviation = "DIE" };
        private static readonly FuelType SuperType = new() { Id = 2, DisplayValue = "Super", Abbreviation = "SUP" };
        private static readonly Currency EurCurrency = new() { Id = 1, Name = "Euro", Abbreviation = "EUR", Symbol = "€" };

        private static (EControlCollectorService Service, Mock<IFuelTypeRepository> FuelTypeRepo, Mock<ICurrencyRepository> CurrencyRepo)
            BuildService(HttpMessageHandler handler, string baseFetchAddress = "https://api.e-control.at")
        {
            var configMock = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["BaseFetchAddress"] = baseFetchAddress,
                    ["FetchSettings:IntervalUnit"] = "Hour",
                    ["FetchSettings:IntervalValue"] = "1",
                })
                .Build();

            var loggerMock = new Mock<ILogger<EControlCollectorService>>();
            var fuelTypeRepoMock = new Mock<IFuelTypeRepository>();
            var currencyRepoMock = new Mock<ICurrencyRepository>();

            fuelTypeRepoMock.Setup(r => r.GetAllAsync(default))
                .ReturnsAsync(new List<FuelType> { DieselType, SuperType });

            currencyRepoMock.Setup(r => r.GetAllAsync(default))
                .ReturnsAsync(new List<Currency> { EurCurrency });

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(baseFetchAddress) };

            var service = new EControlCollectorService(
                configMock, httpClient, loggerMock.Object,
                fuelTypeRepoMock.Object, currencyRepoMock.Object);

            return (service, fuelTypeRepoMock, currencyRepoMock);
        }

        private static HttpMessageHandler BuildResponseHandler(object body, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new FakeHttpMessageHandler(new HttpResponseMessage(status)
            {
                Content = new StringContent(JsonSerializer.Serialize(body))
            });
        }

        [Fact]
        public async Task FetchPricesByLocationAndFuelTypeAsync_HappyPath_ReturnsPrices()
        {
            var responseBody = new[]
            {
                new EControlGasStation
                {
                    Id = 1,
                    Name = "Test Station",
                    Prices = [new EControlPriceReading { FuelType = "DIE", Amount = 1.599m }]
                }
            };
            var (service, _, _) = BuildService(BuildResponseHandler(responseBody));

            var prices = await service.FetchPricesByLocationAsync(48.2m, 16.3m);

            Assert.NotEmpty(prices);
            var price = prices.First();
            Assert.Equal(1.599m, price.Value);
            Assert.Equal(DieselType.Id, price.FuelTypeId);
            Assert.Equal(EurCurrency.Id, price.CurrencyId);
        }

        [Fact]
        public async Task FetchPricesByLocationAndFuelTypeAsync_HttpError_ReturnsEmpty()
        {
            var (service, _, _) = BuildService(BuildResponseHandler(new { }, HttpStatusCode.InternalServerError));

            var prices = await service.FetchPricesByLocationAsync(48.2m, 16.3m);

            Assert.Empty(prices);
        }

        [Fact]
        public async Task FetchPricesByLocationAndFuelTypeAsync_UnsupportedFuelType_ReturnsEmpty()
        {
            var responseBody = new[]
            {
                new EControlGasStation
                {
                    Id = 1,
                    Name = "Test Station",
                    Prices = [new EControlPriceReading { FuelType = "GAS", Amount = 2.0m }]
                }
            };
            var (service, _, _) = BuildService(BuildResponseHandler(responseBody));

            var prices = await service.FetchPricesByLocationAsync(48.2m, 16.3m);

            Assert.Empty(prices);
        }

        [Fact]
        public async Task FetchPricesByLocationAndFuelTypeAsync_MultipleStationsInResponse_UsesGeometricallyNearestStation()
        {
            // Station 2 is first in the list but station 1 is geographically closer.
            // The service must select by minimum distance, not by list position.
            const decimal queryLat = 48.2m;
            const decimal queryLon = 16.3m;

            var responseBody = new[]
            {
                new EControlGasStation
                {
                    Id = 2,
                    Location = new EControlLocation { Latitude = 48.25, Longitude = 16.35 }, // farther
                    Prices = [new EControlPriceReading { FuelType = "DIE", Amount = 1.6m }]
                },
                new EControlGasStation
                {
                    Id = 1,
                    Location = new EControlLocation { Latitude = 48.20, Longitude = 16.30 }, // closer
                    Prices = [new EControlPriceReading { FuelType = "DIE", Amount = 1.5m }]
                }
            };

            var (service, _, _) = BuildService(BuildResponseHandler(responseBody));

            var prices = (await service.FetchPricesByLocationAndFuelTypeAsync(
                queryLat, queryLon, FuelPriceWizard.BusinessLogic.Modules.Enums.FuelType.Diesel)).ToList();

            // Station 1 (closer, second in list) should win — its 1.5 price, not station 2's 1.6.
            Assert.Single(prices);
            Assert.Equal(1.5m, prices[0].Value);
        }

        [Fact]
        public async Task FetchPricesByLocationAndFuelTypeAsync_FetchedAt_IsSet()
        {
            var before = DateTime.UtcNow;
            var responseBody = new[]
            {
                new EControlGasStation
                {
                    Prices = [new EControlPriceReading { FuelType = "SUP", Amount = 1.8m }]
                }
            };
            var (service, _, _) = BuildService(BuildResponseHandler(responseBody));

            var prices = await service.FetchPricesByLocationAsync(48.2m, 16.3m);
            var after = DateTime.UtcNow;

            foreach (var p in prices)
            {
                Assert.InRange(p.FetchedAt, before, after);
            }
        }
    }

    internal sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(HttpResponseMessage response)
            : this(_ => response) { }

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(_handler(request));
    }
}
