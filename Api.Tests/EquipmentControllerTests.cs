using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class EquipmentControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public EquipmentControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Testowa autoryzacja (Admin/Manager/User)
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfEquipment()
    {
        // Arrange
        var equipments = new List<EquipmentDto>
        {
            new() { Id = 1, Name = "Laptop Dell", Type = "Laptop", SerialNumber = "ABC123", Location = "Magazyn A", Status = EquipmentStatus.Dostępny },
            new() { Id = 2, Name = "Monitor LG", Type = "Monitor", SerialNumber = "XYZ999", Location = "Magazyn B", Status = EquipmentStatus.Wypożyczony }
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.GetAllEquipment())
            .Returns(equipments);

        // Act
        var response = await _client.GetAsync("/api/equipment");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var result = await response.Content.ReadFromJsonAsync<List<EquipmentDto>>(options);
        result.Should().BeEquivalentTo(equipments);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithEquipment()
    {
        // Arrange
        var equipment = new EquipmentDto
        {
            Id = 1,
            Name = "Laptop Dell",
            Type = "Laptop",
            SerialNumber = "ABC123",
            Location = "Magazyn A",
            Status = EquipmentStatus.Dostępny
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.GetEquipmentById(1))
            .Returns(equipment);

        // Act
        var response = await _client.GetAsync("/api/equipment/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var result = await response.Content.ReadFromJsonAsync<EquipmentDto>(options);
        result.Should().BeEquivalentTo(equipment);
    }

    [Fact]
    public async Task CreateEquipment_ShouldReturnCreated()
    {
        // Arrange
        var dto = new AddEquipmentDto
        {
            Name = "Drukarka HP",
            Type = "Drukarka",
            SerialNumber = "PRN001",
            Specification = "Laserowa",
            Location = "Biuro 1",
            Status = EquipmentStatus.Dostępny
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.CreateEquipment(It.IsAny<AddEquipmentDto>()))
            .Returns(123);

        // Act
        var response = await _client.PostAsJsonAsync("/api/equipment", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be("/api/equipment/123");
    }

    [Fact]
    public async Task UpdateEquipment_ShouldReturnOk()
    {
        // Arrange
        var dto = new UpdateEquipmentDto
        {
            Name = "Drukarka HP Updated",
            Type = "Drukarka",
            SerialNumber = "PRN001",
            Specification = "Laserowa Duplex",
            Location = "Biuro 2",
            Status = EquipmentStatus.Wypożyczony
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.UpdateEquipment(It.IsAny<UpdateEquipmentDto>(), 1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/equipment/1", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateEquipmentStatus_ShouldReturnOk()
    {
        // Arrange
        var dto = new UpdateEquipmentStatusDto
        {
            Status = EquipmentStatus.Wypożyczony
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.UpdateEquipmentStatus(It.IsAny<UpdateEquipmentStatusDto>(), 1));

        // Act
        var response = await _client.PatchAsJsonAsync("/api/equipment/1", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteEquipment_ShouldReturnNoContent()
    {
        // Arrange
        _factory.EquipmentServiceMock
            .Setup(s => s.DeleteEquipment(1));

        // Act
        var response = await _client.DeleteAsync("/api/equipment/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.EquipmentServiceMock.Verify(s => s.DeleteEquipment(1), Times.Once);
    }

    [Fact]
    public async Task FilterEquipment_ShouldReturnOk_WithFilteredList()
    {
        // Arrange
        var filter = new EquipmentFilterDto { Type = "Laptop" };
        var filteredEquipments = new List<EquipmentDto>
        {
            new() { Id = 1, Name = "Laptop Dell", Type = "Laptop", SerialNumber = "ABC123", Location = "Magazyn A", Status = EquipmentStatus.Dostępny }
        };

        _factory.EquipmentServiceMock
            .Setup(s => s.FilterEquipment(It.IsAny<EquipmentFilterDto>()))
            .Returns(filteredEquipments);

        // Act
        var response = await _client.GetAsync("/api/equipment/search?Type=Laptop");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var result = await response.Content.ReadFromJsonAsync<List<EquipmentDto>>(options);
        result.Should().BeEquivalentTo(filteredEquipments);
    }
}