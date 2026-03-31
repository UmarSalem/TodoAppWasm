using Application.DAO_interfaces;
using Application.LogicImplementations;
using Application.LogicInterfaces;
using EfcDataAccess.DAOs;
using FileData;
using Application.DAOInterfaces;
using EfcDataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<FileContext>();
builder.Services.AddScoped<IUserDao, UserEfcDao>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<ITodoLogic, TodoLogic>();
builder.Services.AddScoped<ITodoDao, TodoEfcDao>();
builder.Services.AddDbContext<TodoContext>();

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

app.UseCors("FrontendPolicy");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
