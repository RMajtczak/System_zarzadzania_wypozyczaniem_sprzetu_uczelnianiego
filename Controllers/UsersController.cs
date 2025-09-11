using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Services;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    [HttpPut("update-role/{userId}")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] RoleUpdateDto roleUpdateDto)
    {
        var result = await _userService.UpdateUserRoleAsync(userId, roleUpdateDto.RoleId);
        if (!result) return NotFound("Użytkownik lub rola nie została znaleziona.");

        return Ok("Rola użytkownika została zaktualizowana.");
    }
}