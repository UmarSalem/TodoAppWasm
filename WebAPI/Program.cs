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
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
var todoDatabaseConnectionString = builder.Configuration.GetConnectionString("TodoDatabase")
    ?? "Data Source=../EfcDataAccess/Todo.db";

// The provider is configurable so local development can stay on SQLite,
// while the deployed API can use PostgreSQL without changing code.
builder.Services.AddDbContext<TodoContext>(options =>
{
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
        || databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(todoDatabaseConnectionString);
        return;
    }

    if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase)
        || databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(todoDatabaseConnectionString);
        return;
    }

    throw new InvalidOperationException(
        $"Unsupported database provider '{databaseProvider}'. Use 'Sqlite' or 'Postgres'.");
});

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
