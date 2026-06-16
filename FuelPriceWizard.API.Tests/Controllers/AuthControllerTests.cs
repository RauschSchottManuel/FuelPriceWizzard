using FuelPriceWizard.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FuelPriceWizard.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private static AuthController BuildController(string adminPassword)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:AdminPassword"] = adminPassword,
                    ["JwtSettings:Secret"] = "super-secret-key-that-is-at-least-32-chars!",
                    ["JwtSettings:Issuer"] = "TestIssuer",
                    ["JwtSettings:Audience"] = "TestAudience",
                    ["JwtSettings:ExpirationMinutes"] = "60",
                })
                .Build();

            return new AuthController(config);
        }

        [Fact]
        public void GetToken_ValidPassword_ReturnsOkWithToken()
        {
            var controller = BuildController("correct-password");

            var result = controller.GetToken(new AuthController.LoginRequest("correct-password"));

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            var prop = ok.Value!.GetType().GetProperty("token");
            Assert.NotNull(prop);
            var token = prop!.GetValue(ok.Value) as string;
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GetToken_InvalidPassword_ReturnsUnauthorized()
        {
            var controller = BuildController("correct-password");

            var result = controller.GetToken(new AuthController.LoginRequest("wrong-password"));

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GetToken_EmptyPassword_ReturnsUnauthorized()
        {
            var controller = BuildController("correct-password");

            var result = controller.GetToken(new AuthController.LoginRequest(string.Empty));

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
