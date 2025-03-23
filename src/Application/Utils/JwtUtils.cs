using dotnet_api_erp.src.Domain.Entities.UserContext;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static dotnet_api_erp.src.Application.Utils.JwtUtils;


namespace dotnet_api_erp.src.Application.Utils
{
    public class JwtUtils(IOptions<JwtSettingsUseCase> jwtSettings)
    {
        private readonly string secureKey = jwtSettings.Value.Key;

        public RefreshToken GenerateToken(Guid id)
        {
            var key = Encoding.ASCII.GetBytes(secureKey);
            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("Id", id.ToString()),
                ]),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            string tokenString = tokenHandler.WriteToken(token);
            return new RefreshToken(id, tokenString);
        }
        public bool ValidadeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secureKey);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public class JwtSettingsUseCase
        {
            public required string Key { get; set; }
        }
    }
}