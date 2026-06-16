using FuelPriceWizard.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace FuelPriceWizard.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetGasStations_Unauthenticated_Returns200()
        {
            var response = await _client.GetAsync("/api/gasstations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateGasStation_Unauthenticated_Returns401()
        {
            var dto = new { designation = "Test Station" };
            var response = await _client.PostAsJsonAsync("/api/gasstations", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetToken_ValidPassword_Returns200WithToken()
        {
            var request = new { password = "integration-test-password" };
            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.False(string.IsNullOrWhiteSpace(body?.Token));
        }

        [Fact]
        public async Task GetToken_InvalidPassword_Returns401()
        {
            var request = new { password = "wrong-password" };
            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetFuelTypes_Unauthenticated_Returns200()
        {
            var response = await _client.GetAsync("/api/fueltypes");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateFuelType_Unauthenticated_Returns401()
        {
            var dto = new { displayValue = "Diesel", abbreviation = "DIE", isActive = true };
            var response = await _client.PostAsJsonAsync("/api/fueltypes", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private record TokenResponse(string Token);
    }

    public class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<FuelPriceWizardDbContext>>();
                services.RemoveAll<FuelPriceWizardDbContext>();

                services.AddDbContext<FuelPriceWizardDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestDb"));

                services.Configure<Microsoft.Extensions.Options.IOptions<object>>(_ => { });
            });

            builder.UseSetting("JwtSettings:AdminPassword", "integration-test-password");
            builder.UseSetting("JwtSettings:Secret", "integration-test-secret-key-minimum-32-chars!!");
            builder.UseSetting("JwtSettings:Issuer", "TestIssuer");
            builder.UseSetting("JwtSettings:Audience", "TestAudience");
            builder.UseSetting("JwtSettings:ExpirationMinutes", "60");
        }
    }
}
