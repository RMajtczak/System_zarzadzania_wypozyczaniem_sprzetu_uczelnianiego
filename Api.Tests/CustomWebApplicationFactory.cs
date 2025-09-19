using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Wypożyczlania_sprzętu;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Services;

namespace Api.Tests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                               ILoggerFactory logger,
                               UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.NameIdentifier, "42"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public readonly Mock<IUserService> UserServiceMock = new();
        public readonly Mock<IAccountService> AccountServiceMock = new();
        public readonly Mock<IBorrowingService> BorrowingServiceMock = new();
        public readonly Mock<IEquipmentService> EquipmentServiceMock = new();
        public readonly Mock<IFaultReportService> FaultReportServiceMock = new();
        public readonly Mock<IReservationService> ReservationServiceMock = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test"); // środowisko testowe
            
            // Konfiguracja logowania, aby uniknąć konfliktu z Serilogiem Aspire
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddFilter("Microsoft", LogLevel.Warning);
                logging.AddFilter("System", LogLevel.Warning);
            });

            builder.ConfigureServices(services =>
            {
                // Zamieniamy DbContext na InMemory dla testów
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<RentalDbContext>));
                if (descriptor != null) services.Remove(descriptor);
             
            });

            builder.ConfigureTestServices(services =>
            {
                RemoveService<IUserService>(services);
                RemoveService<IAccountService>(services);
                RemoveService<IBorrowingService>(services);
                RemoveService<IEquipmentService>(services);
                RemoveService<IFaultReportService>(services);
                RemoveService<IReservationService>(services);
                
                services.AddScoped(_ => UserServiceMock.Object);
                services.AddScoped(_ => AccountServiceMock.Object);
                services.AddScoped(_ => BorrowingServiceMock.Object);
                services.AddScoped(_ => EquipmentServiceMock.Object);
                services.AddScoped(_ => FaultReportServiceMock.Object);
                services.AddScoped(_ => ReservationServiceMock.Object);
                
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }

        private void RemoveService<T>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
            if (descriptor != null)
                services.Remove(descriptor);
        }
    }
}