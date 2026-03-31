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

var apiBase = builder.Configuration["ApiBase"];
if (string.IsNullOrWhiteSpace(apiBase))
{
    apiBase = "http://localhost:5112/";
}

builder.Services.AddHttpClient<IUserService, UserHttpClient>(c =>
    c.BaseAddress = new Uri(apiBase));

builder.Services.AddHttpClient<ITodoService, TodoHttpClient>(c =>
    c.BaseAddress = new Uri(apiBase));

await builder.Build().RunAsync();
