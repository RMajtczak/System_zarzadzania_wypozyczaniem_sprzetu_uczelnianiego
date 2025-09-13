using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class FaultReportControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public FaultReportControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Testowa autoryzacja (Admin/Manager/User)
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfFaultReports()
    {
        // Arrange
        var reports = new List<FaultReportDto>
        {
            new() { Id = 1, EquipmentId = 1, UserName = "Jan", EquipmentName = "Laptop Dell", Description = "Nie działa", ReportDate = System.DateTime.Now, IsResolved = false },
            new() { Id = 2, EquipmentId = 2, UserName = "Anna", EquipmentName = "Monitor LG", Description = "Popsuty kabel", ReportDate = System.DateTime.Now, IsResolved = true }
        };

        _factory.FaultReportServiceMock
            .Setup(s => s.GetAllFaultReports())
            .Returns(reports);

        // Act
        var response = await _client.GetAsync("/api/faultreports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FaultReportDto>>();
        result.Should().BeEquivalentTo(reports);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithFaultReport()
    {
        // Arrange
        var report = new FaultReportDto
        {
            Id = 1,
            EquipmentId = 1,
            UserName = "Jan",
            EquipmentName = "Laptop Dell",
            Description = "Nie działa",
            ReportDate = System.DateTime.Now,
            IsResolved = false
        };

        _factory.FaultReportServiceMock
            .Setup(s => s.GetFaultReportById(1))
            .Returns(report);

        // Act
        var response = await _client.GetAsync("/api/faultreports/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<FaultReportDto>();
        result.Should().BeEquivalentTo(report);
    }

    [Fact]
    public async Task CreateFaultReport_ShouldReturnCreated()
    {
        // Arrange
        var dto = new AddFaultReportDto
        {
            UserName = "Jan",
            EquipmentName = "Laptop Dell",
            Description = "Nie działa"
        };

        _factory.FaultReportServiceMock
            .Setup(s => s.CreateFaultReport(It.Is<AddFaultReportDto>(x =>
                x.UserName == "Jan" && x.EquipmentName == "Laptop Dell")))
            .Returns(123);

        // Act
        var response = await _client.PostAsJsonAsync("/api/faultreports", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be("/api/faultreports/123");
    }

    [Fact]
    public async Task ResolveFaultReport_ShouldReturnOk()
    {
        // Arrange
        _factory.FaultReportServiceMock
            .Setup(s => s.ResolveFaultReport(1));

        // Act
        var response = await _client.PatchAsync("/api/faultreports/1/resolve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.FaultReportServiceMock.Verify(s => s.ResolveFaultReport(1), Times.Once);
    }
}
