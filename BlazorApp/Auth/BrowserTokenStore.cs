using Microsoft.JSInterop;

namespace BlazorApp.Auth
{
    public class BrowserTokenStore : ITokenStore
    {
        private const string TokenKey = "todoapp.jwt";
        private readonly IJSRuntime jsRuntime;

        public BrowserTokenStore(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }

        public async Task SetTokenAsync(string token)
        {
            // localStorage keeps the login after refresh. This is acceptable for this portfolio app,
            // but production apps may choose stricter storage depending on their threat model.
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task ClearTokenAsync()
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }
}
