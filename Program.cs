using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
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

    var authenticationSettings = new AutenticationSettings();
    builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
    builder.Services.AddSingleton(authenticationSettings);

    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    }).AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
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
    builder.Services.AddScoped<IEquipmentService, EquipmentService>();
    builder.Services.AddScoped<IBorrowingService, BorrowingService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddHostedService<ReservationBackgroundService>();
    builder.Services.AddScoped<IFaultReportService, FaultReportService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(typeof(Program));

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
    
    //app.MapControllerRoute(
            //name: "default",
            //pattern: "{controller=Home}/{action=Index}/{id?}")
        //.WithStaticAssets();
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
