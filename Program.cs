using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Wypożyczlania_sprzętu;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Middleware;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Models.validators;
using Wypożyczlania_sprzętu.Services;
using Prometheus;

var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings() 
    .GetCurrentClassLogger();

try
{
    logger.Info("Start aplikacji");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    //var authenticationSettings = new AutenticationSettings();
    //builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
    //builder.Services.AddSingleton(authenticationSettings);

    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,       // Kto wydał token (np. "your-api")
            ValidateAudience = true,     // Dla kogo jest token (np. "your-client")
            ValidateLifetime = true,     // Czy sprawdzać czas życia tokenu
            ValidateIssuerSigningKey = true, // Czy walidować klucz
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend",
            policy => policy
                .WithOrigins("http://localhost:5173", "https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    builder.Services.AddDbContext<RentalDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<EquipmentSeeder>();
    builder.Services.AddScoped<IReportService, ReportService>();
    builder.Services.AddScoped<IEquipmentService, EquipmentService>();
    builder.Services.AddScoped<IBorrowingService, BorrowingService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddHostedService<ReservationBackgroundService>();
    builder.Services.AddScoped<IFaultReportService, FaultReportService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(typeof(Program));

    // Dodaj Prometheus
    builder.Services.AddSingleton<CollectorRegistry>(); // rejestr dla metryk

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<EquipmentSeeder>();
        seeder.Seed();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseRouting();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wypożyczalnia Sprzętu API ");
    });

    // Prometheus middleware
    app.UseHttpMetrics(); // automatyczne liczenie requestów i czasów
    app.MapMetrics("/metrics"); // endpoint /metrics dla Prometheus

    // Health Check endpoint
    app.MapGet("/health", async (RentalDbContext db) =>
    {
        try
        {
            bool dbOk = await db.Database.CanConnectAsync();
            if (dbOk)
                return Results.Ok(new { status = "Healthy", db = dbOk });
            else
                return Results.Json(new { status = "Unhealthy", db = dbOk }, statusCode: 503);
        }
        catch
        {
            return Results.Json(new { status = "Unhealthy", db = false }, statusCode: 503);
        }
    });

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Błąd krytyczny podczas uruchamiania aplikacji");
    throw;
}
finally
{
    LogManager.Shutdown(); 
}
