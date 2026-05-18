using System.Net.Http.Headers;

namespace BlazorApp.Auth
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ITokenStore tokenStore;

        public JwtAuthorizationMessageHandler(ITokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = await tokenStore.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                // Every typed API client goes through this handler, so protected endpoints
                // automatically receive the same Bearer token after login.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
