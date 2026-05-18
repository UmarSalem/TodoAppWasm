using BlazorApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HttpClients.ClientInterfaces;
using HttpClients.Implementations;
using BlazorApp.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStore, BrowserTokenStore>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();

string apiBase = builder.Configuration["ApiBase"] ?? "";
if (string.IsNullOrWhiteSpace(apiBase))
{
    apiBase = "https://localhost:7161/";
}

if (!Uri.TryCreate(apiBase, UriKind.Absolute, out Uri? apiBaseAddress))
{
    throw new InvalidOperationException("ApiBase must be an absolute URL, for example https://your-api.onrender.com/.");
}

// Keep one normalized API base address for all typed HTTP clients.
// Local development uses appsettings.json; GitHub Pages receives the Render URL from the API_BASE_URL secret.
if (!apiBaseAddress.AbsoluteUri.EndsWith('/'))
{
    apiBaseAddress = new Uri(apiBaseAddress.AbsoluteUri + "/");
}

builder.Services.AddHttpClient<IUserService, UserHttpClient>(c =>
    c.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ITodoService, TodoHttpClient>(c =>
    c.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

await builder.Build().RunAsync();
