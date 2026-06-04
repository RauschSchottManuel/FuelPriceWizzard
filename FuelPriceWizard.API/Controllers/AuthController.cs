using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public record LoginRequest(string Password);

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetToken([FromBody] LoginRequest request)
        {
            var expectedPassword = configuration["JwtSettings:AdminPassword"];
            if (string.IsNullOrEmpty(expectedPassword) || request.Password != expectedPassword)
                return Unauthorized();

            var secret = configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = int.TryParse(configuration["JwtSettings:ExpirationMinutes"], out var minutes)
                ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: [new Claim(ClaimTypes.Role, "Admin")],
                expires: DateTime.UtcNow.AddMinutes(expiration),
                signingCredentials: credentials);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
