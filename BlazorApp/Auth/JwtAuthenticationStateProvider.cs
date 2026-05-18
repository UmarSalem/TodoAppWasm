using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());
        private readonly ITokenStore tokenStore;

        public JwtAuthenticationStateProvider(ITokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = await tokenStore.GetTokenAsync();
            ClaimsPrincipal user = string.IsNullOrWhiteSpace(token)
                ? Anonymous
                : BuildPrincipalFromToken(token);

            return new AuthenticationState(user);
        }

        public void NotifyUserChanged()
        {
            // Blazor rerenders AuthorizeView/NavMenu when the login state changes.
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static ClaimsPrincipal BuildPrincipalFromToken(string token)
        {
            try
            {
                string payload = token.Split('.')[1];
                byte[] jsonBytes = ParseBase64WithoutPadding(payload);
                Dictionary<string, JsonElement>? claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);

                if (claims == null || IsExpired(claims))
                {
                    return Anonymous;
                }

                ClaimsIdentity identity = new(MapClaims(claims), "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return Anonymous;
            }
        }

        private static IEnumerable<Claim> MapClaims(Dictionary<string, JsonElement> claims)
        {
            foreach ((string key, JsonElement value) in claims)
            {
                string claimType = key switch
                {
                    "nameid" => ClaimTypes.NameIdentifier,
                    "unique_name" => ClaimTypes.Name,
                    "role" => ClaimTypes.Role,
                    _ => key
                };

                if (value.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in value.EnumerateArray())
                    {
                        yield return new Claim(claimType, item.ToString());
                    }
                }
                else
                {
                    yield return new Claim(claimType, value.ToString());
                }
            }
        }

        private static bool IsExpired(Dictionary<string, JsonElement> claims)
        {
            if (!claims.TryGetValue("exp", out JsonElement expiryValue))
            {
                return false;
            }

            long expirySeconds = expiryValue.GetInt64();
            DateTimeOffset expiry = DateTimeOffset.FromUnixTimeSeconds(expirySeconds);
            return expiry <= DateTimeOffset.UtcNow;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            string padded = base64.Replace('-', '+').Replace('_', '/');
            padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
            return Convert.FromBase64String(padded);
        }
    }
}
