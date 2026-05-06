using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using Shared.Models;

namespace WebAPI.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UserLoginResponseDto CreateLoginResponse(User user)
        {
            DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(GetTokenLifetimeMinutes());
            string token = CreateToken(user, expiresAtUtc);
            UserReadDto safeUser = new(user.Id, user.UserName, user.Role);
            return new UserLoginResponseDto(token, safeUser, expiresAtUtc);
        }

        private string CreateToken(User user, DateTime expiresAtUtc)
        {
            // Claims are the small pieces of identity data we trust after login.
            // Later, Todo endpoints read NameIdentifier instead of trusting ownerId from the request body.
            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            ];

            SymmetricSecurityKey key = new(GetSigningKey());
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private byte[] GetSigningKey()
        {
            string? signingKey = configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
            {
                throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters long.");
            }

            return Encoding.UTF8.GetBytes(signingKey);
        }

        private int GetTokenLifetimeMinutes()
        {
            int configuredMinutes = configuration.GetValue<int?>("Jwt:TokenLifetimeMinutes") ?? 60;
            return Math.Clamp(configuredMinutes, 5, 24 * 60);
        }
    }
}
