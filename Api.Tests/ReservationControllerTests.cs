using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class ReservationControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReservationControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Testowa autoryzacja z rolą Admin/User
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task GetOwn_ShouldReturnOk_WithUserReservations()
    {
        // Arrange
        var userId = 42;
        var reservations = new List<ReservationDto>
        {
            new() { Id = 1, EquipmentId = 1, EquipmentName = "Laptop Dell", BookerName = "Jan", StartDate = System.DateTime.Now, EndDate = null },
            new() { Id = 2, EquipmentId = 2, EquipmentName = "Monitor LG", BookerName = "Jan", StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddDays(1) }
        };

        _factory.ReservationServiceMock
            .Setup(s => s.GetReservationsByUserId(userId))
            .Returns(reservations);

        // Aby User.Claims miał NameIdentifier
        _client.DefaultRequestHeaders.Add("Test-UserId", userId.ToString());

        // Act
        var response = await _client.GetAsync("/api/reservations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();
        result.Should().BeEquivalentTo(reservations);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithReservation()
    {
        // Arrange
        var reservation = new ReservationDto
        {
            Id = 1,
            EquipmentId = 1,
            EquipmentName = "Laptop Dell",
            BookerName = "Jan",
            StartDate = System.DateTime.Now,
            EndDate = null
        };

        _factory.ReservationServiceMock
            .Setup(s => s.GetReservationById(1))
            .Returns(reservation);

        // Act
        var response = await _client.GetAsync("/api/reservations/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ReservationDto>();
        result.Should().BeEquivalentTo(reservation);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateReservationDto
        {
            EquipmentName = "Laptop Dell",
            BookerName = "Jan",
            StartDate = System.DateTime.Now,
            EndDate = System.DateTime.Now.AddDays(1)
        };

        _factory.ReservationServiceMock
            .Setup(s => s.CreateReservation(
                It.IsAny<CreateReservationDto>(),
                It.IsAny<string>()))
            .Returns(123);

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be("/api/reservations/123");
    }

    [Fact]
    public async Task CancelReservation_ShouldReturnOk()
    {
        // Arrange
        _factory.ReservationServiceMock
            .Setup(s => s.CancelReservation(1));

        // Act
        var response = await _client.PostAsync("/api/reservations/1/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.ReservationServiceMock.Verify(s => s.CancelReservation(1), Times.Once);
    }
}