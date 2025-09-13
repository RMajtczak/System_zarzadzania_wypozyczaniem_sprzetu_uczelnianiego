using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Wypożyczlania_sprzętu.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;

namespace Api.Tests;

// Ta klasa służy do obejścia prawdziwej autoryzacji podczas testów.
// Tworzy użytkownika z rolą "Admin", aby testy mogły przejść przez [Authorize(Roles = "Admin")].
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { 
            new Claim(ClaimTypes.Name, "Test user"),
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Role, "Admin"),
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
        builder.ConfigureTestServices(services =>
        {
            
            var userDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
            if (userDescriptor != null) services.Remove(userDescriptor);

            var accountDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAccountService));
            if (accountDescriptor != null) services.Remove(accountDescriptor);
            
            var borrowingDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBorrowingService));
            if (borrowingDescriptor != null) services.Remove(borrowingDescriptor);
            
            var equipmentDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEquipmentService));
            if (equipmentDescriptor != null) services.Remove(equipmentDescriptor);
            
            var faultReportDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFaultReportService));
            if (faultReportDescriptor != null) services.Remove(faultReportDescriptor);
            
            var reservationDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IReservationService));
            if (reservationDescriptor != null) services.Remove(reservationDescriptor);

            // Dodaj mocki
            services.AddScoped(_ => UserServiceMock.Object);
            services.AddScoped(_ => AccountServiceMock.Object);
            services.AddScoped(_ => BorrowingServiceMock.Object);
            services.AddScoped(_ => EquipmentServiceMock.Object);
            services.AddScoped(_ => FaultReportServiceMock.Object);
            services.AddScoped(_ => ReservationServiceMock.Object);
            
            // Autoryzacja testowa
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });
    }
}

