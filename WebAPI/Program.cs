using Application.DAO_interfaces;
using Application.LogicImplementations;
using Application.LogicInterfaces;
using EfcDataAccess.DAOs;
using FileData;
using Application.DAOInterfaces;
using EfcDataAccess;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<FileContext>();
// Register interfaces against concrete classes so controllers depend on abstractions,
// while Program.cs decides whether the app uses EF Core, file storage, or another data source.
builder.Services.AddScoped<IUserDao, UserEfcDao>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<ITodoLogic, TodoLogic>();
builder.Services.AddScoped<ITodoDao, TodoEfcDao>();

var todoDatabaseConnectionString = builder.Configuration.GetConnectionString("TodoDatabase")
    ?? "Data Source=../EfcDataAccess/Todo.db";

// Keep the database location in configuration so local development, Docker,
// and later cloud databases can each use their own connection string.
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite(todoDatabaseConnectionString));

// Hosted containers usually sit behind a reverse proxy that terminates HTTPS.
// This lets ASP.NET Core understand the original public request scheme and client IP.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var allowedOrigins = builder.Configuration
    .GetValue<string>("AllowedOrigins")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }
    });
});

var app = builder.Build();

app.UseForwardedHeaders();

// Convert domain/application exceptions into consistent HTTP responses in one place.
// This keeps controllers focused on request/response flow instead of repeating try/catch blocks.
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseCors("FrontendPolicy");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

// Hosting platforms can call this endpoint to confirm the API process is alive.
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();
