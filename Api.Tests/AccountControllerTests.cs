using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AccountControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsOk()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Jan",
            LastName = "Kowalski",
            RoleId = 1
        };

        _factory.AccountServiceMock
            .Setup(s => s.RegisterUser(It.IsAny<RegisterUserDto>()));

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.AccountServiceMock.Verify(s => s.RegisterUser(It.IsAny<RegisterUserDto>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUserName()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "admin@example.com",
            Password = "Password123!"
        };

        var tokenResponse = new TokenResponseDto
        {
            Token = "fake-jwt-token",
            UserName = "AdminUser"
        };

        _factory.AccountServiceMock
            .Setup(s => s.GenerateToken(It.IsAny<LoginDto>()))
            .Returns(tokenResponse);

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        result.Should().NotBeNull();
        result.Token.Should().Be(tokenResponse.Token);
        result.UserName.Should().Be(tokenResponse.UserName);
    }
}