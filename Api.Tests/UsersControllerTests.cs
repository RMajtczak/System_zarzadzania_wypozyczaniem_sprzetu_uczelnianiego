using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;
using Xunit;

namespace Api.Tests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult_WithUsersList()
    {
        
        var users = new List<UserDto>
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Kowalski" },
            new() { Id = 2, FirstName = "Anna", LastName = "Nowak" }
        };
        _factory.UserServiceMock
            .Setup(s => s.GetAllUsersAsync())
            .ReturnsAsync(users);

        
        var response = await _client.GetAsync("/api/users");

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedUsers = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        returnedUsers.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task UpdateUserRole_WithValidData_ReturnsOk()
    {
        
        var userId = 1;
        var roleUpdateDto = new RoleUpdateDto { RoleId = 2 };
        _factory.UserServiceMock
            .Setup(s => s.UpdateUserRoleAsync(userId, roleUpdateDto.RoleId))
            .ReturnsAsync(true);

        
        var response = await _client.PutAsJsonAsync($"/api/users/update-role/{userId}", roleUpdateDto);

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserRole_WithNonExistentUser_ReturnsNotFound()
    {
        
        var userId = 999;
        var roleUpdateDto = new RoleUpdateDto { RoleId = 2 };
        _factory.UserServiceMock
            .Setup(s => s.UpdateUserRoleAsync(userId, roleUpdateDto.RoleId))
            .ReturnsAsync(false);

        
        var response = await _client.PutAsJsonAsync($"/api/users/update-role/{userId}", roleUpdateDto);

        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
