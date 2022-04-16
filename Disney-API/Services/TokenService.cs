using DotEnv.Core;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Disney_API.Services
{
    public class TokenService : ITokenService
    {

        private readonly EnvReader reader;
        public TokenService()
        {
            reader = new EnvReader();
        }

        public string GetToken(string Email)
        {
            var secretKey = reader["SecretKey"];
            Debug.WriteLine(secretKey);
            var key = Encoding.ASCII.GetBytes(secretKey!);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, Email));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(6), //6 horas de validez del token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(createdToken);
        }
    }
}
