using Application.DAO_interfaces;
using Application.LogicImplementations;
using Application.LogicInterfaces;
using EfcDataAccess.DAOs;
using FileData;
using Application.DAOInterfaces;
using EfcDataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth;
using Shared.DTOs;
using System.Text;
using WebAPI.Auth;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // [ApiController] can stop bad requests before they reach our logic layer.
        // Returning ApiErrorDto here keeps validation errors shaped like the rest of the API.
        options.InvalidModelStateResponseFactory = context =>
        {
            string message = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "The request body is invalid."
                    : error.ErrorMessage)
                .FirstOrDefault()
                ?? "The request body is invalid.";

            return new BadRequestObjectResult(new ApiErrorDto(message));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Swagger needs this security definition before it can show the Authorize button.
    // The value users paste is: Bearer <JWT token from /Users/login>.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here. Swagger will send it as: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<FileContext>();
// Register interfaces against concrete classes so controllers depend on abstractions,
// while Program.cs decides whether the app uses EF Core, file storage, or another data source.
builder.Services.AddScoped<IUserDao, UserEfcDao>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<ITodoLogic, TodoLogic>();
builder.Services.AddScoped<ITodoDao, TodoEfcDao>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is required.");

// Authentication checks the JWT signature and turns a valid token into User.Claims.
// Authorization then uses those claims to decide whether an endpoint can run.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // JWT middleware normally returns an empty 401 response.
                // This gives Swagger/frontends the same simple error object as the rest of the API.
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorDto("Authentication token is missing or invalid."));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorDto("You are not allowed to perform this action."));
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRoles.Admin));
});

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
        // PostgreSQL has its own migrations assembly because EF Core migrations
        // store provider-specific column types. Keeping it separate avoids applying
        // SQLite migrations to a hosted PostgreSQL database.
        options.UseNpgsql(todoDatabaseConnectionString, providerOptions =>
            providerOptions.MigrationsAssembly("EfcDataAccess.Postgres"));
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

if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    // Hosted databases usually start empty. This optional switch lets the API
    // apply EF Core migrations during deployment, while local development can
    // keep manual control by leaving the setting false.
    using IServiceScope scope = app.Services.CreateScope();
    TodoContext todoContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
    await todoContext.Database.MigrateAsync();
}

app.UseForwardedHeaders();

// Convert domain/application exceptions into consistent HTTP responses in one place.
// This keeps controllers focused on request/response flow instead of repeating try/catch blocks.
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseCors("FrontendPolicy");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Hosting platforms can call this endpoint to confirm the API process is alive.
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();
