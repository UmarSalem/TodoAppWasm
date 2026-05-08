using BlazorApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HttpClients.ClientInterfaces;
using HttpClients.Implementations;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

string apiBase = builder.Configuration["ApiBase"] ?? "";
if (string.IsNullOrWhiteSpace(apiBase))
{
    apiBase = "http://localhost:5112/";
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
    c.BaseAddress = apiBaseAddress);

builder.Services.AddHttpClient<ITodoService, TodoHttpClient>(c =>
    c.BaseAddress = apiBaseAddress);

await builder.Build().RunAsync();
