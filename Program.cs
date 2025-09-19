using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wypożyczlania_sprzętu;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Middleware;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Models.validators;
using Wypożyczlania_sprzętu.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// ZUPEŁNIE USUWAMY KONFIGURACJĘ NLOG! Aspire się tym zajmie.
// if (!builder.Environment.IsEnvironment("Test"))
// {
//     var logger = NLog.LogManager.Setup()
//         .LoadConfigurationFromAppSettings()
//         .GetCurrentClassLogger();
// 
//     builder.Host.UseNLog();
// }
// builder.Logging.ClearProviders();
// builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Authentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
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

// DbContext
if (builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<RentalDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<RentalDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    });
}

// Rejestracja serwisów
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
builder.AddServiceDefaults();

var app = builder.Build();

// Seedowanie danych tylko w normalnym środowisku
if (!builder.Environment.IsEnvironment("Test"))
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<EquipmentSeeder>();
        seeder.Seed();
    }
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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wypożyczalnia Sprzętu API ");
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapDefaultEndpoints();
app.MapControllers();
app.Run();