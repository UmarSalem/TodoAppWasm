using BlazorApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HttpClients.ClientInterfaces;     // IUserService, ITodoService
using HttpClients.Implementations;      // UserHttpClient, TodoHttpClient
using System.Net.Http;                  // add this if ImplicitUsings are off

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Default client for Blazor's own static files (keep this)
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// API base — from config or fallback. Your Swagger shows http://localhost:5112/
var apiBase = builder.Configuration["ApiBase"] ?? "http://localhost:5112/";

// Bind each interface to its HttpClient-based implementation (typed clients)
builder.Services.AddHttpClient<IUserService, UserHttpClient>(c =>
    c.BaseAddress = new Uri(apiBase));

//builder.Services.AddHttpClient<ITodoService, TodoHttpClient>(c =>
//    c.BaseAddress = new Uri(apiBase));

await builder.Build().RunAsync();
