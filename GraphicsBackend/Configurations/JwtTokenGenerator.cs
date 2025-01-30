using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphicsBackend.Configurations
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expirationInMinutes = jwtSettings.GetValue<int>("ExpirationInMinutes");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())            
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
