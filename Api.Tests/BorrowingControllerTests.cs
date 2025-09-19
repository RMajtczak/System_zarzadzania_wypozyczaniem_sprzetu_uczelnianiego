using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class BorrowingControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BorrowingControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Ustawiamy autoryzację testową (Admin)
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfBorrowings()
    {
        // Arrange
        var borrowings = new List<BorrowingDto>
        {
            new() { Id = 50, EquipmentId = 10, EquipmentName = "Laptop", BorrowerName = "Jan Kowalski", StartDate = DateTime.Now },
            new() { Id = 51, EquipmentId = 11, EquipmentName = "Projektor", BorrowerName = "Anna Nowak", StartDate = DateTime.Now }
        };

        _factory.BorrowingServiceMock
            .Setup(s => s.GetAllBorrowings())
            .Returns(borrowings);

        // Act
        var response = await _client.GetAsync("/api/borrowings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
        result.Should().BeEquivalentTo(borrowings);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithBorrowing()
    {
        // Arrange
        var borrowingId = 50;
        var borrowing = new BorrowingDto
        {
            Id = borrowingId,
            EquipmentId = 10,
            EquipmentName = "Laptop",
            BorrowerName = "Jan Kowalski",
            StartDate = DateTime.Now
        };

        _factory.BorrowingServiceMock
            .Setup(s => s.GetBorrowingById(borrowingId))
            .Returns(borrowing);

        // Act
        var response = await _client.GetAsync($"/api/borrowings/{borrowingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BorrowingDto>();
        result.Should().BeEquivalentTo(borrowing);
    }

    [Fact]
    public async Task AddBorrow_ShouldReturnCreated()
    {
        // Arrange
        var dto = new AddBorrowDto
        {
            EquipmentName = "Laptop",
            BorrowerName = "Jan Kowalski",
            Condition = "Dobra",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7)
        };

        var newBorrowingId = 123;

        _factory.BorrowingServiceMock
            .Setup(s => s.AddBorrow(It.IsAny<AddBorrowDto>()))
            .Returns(newBorrowingId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/borrowings", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be($"/api/borrowings/{newBorrowingId}");
    }

    [Fact]
    public async Task DeleteBorrowing_ShouldReturnNoContent()
    {
        // Arrange
        var borrowingId = 1;
        _factory.BorrowingServiceMock
            .Setup(s => s.DeleteBorrowing(borrowingId));

        // Act
        var response = await _client.DeleteAsync($"/api/borrowings/{borrowingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.BorrowingServiceMock.Verify(s => s.DeleteBorrowing(borrowingId), Times.Once);
    }

    [Fact]
    public async Task Return_ShouldReturnOk()
    {
        // Arrange
        var borrowingId = 1;
        _factory.BorrowingServiceMock
            .Setup(s => s.Return(borrowingId));

        // Act
        var response = await _client.PatchAsync($"/api/borrowings/{borrowingId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var msg = await response.Content.ReadAsStringAsync();
        msg.Should().Contain("Zwrócono sprzęt");
    }

    [Fact]
    public async Task GetActiveBorrowings_ShouldReturnOk_WithList()
    {
        // Arrange
        var activeBorrowings = new List<BorrowingDto>
        {
            new() { Id = 50, EquipmentId = 10, EquipmentName = "Laptop", BorrowerName = "Jan Kowalski", StartDate = DateTime.Now, IsReturned = false }
        };

        _factory.BorrowingServiceMock
            .Setup(s => s.GetActiveBorrowings())
            .Returns(activeBorrowings);

        // Act
        var response = await _client.GetAsync("/api/borrowings/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
        result.Should().BeEquivalentTo(activeBorrowings);
    }
}