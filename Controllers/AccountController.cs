using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route("api/account")]
[ApiController]
public class AccountController: ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
    {
        _accountService.RegisterUser(dto);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDto dto)
    {
        var tokenResponse = _accountService.GenerateToken(dto);

        return Ok(new
        {
            token = tokenResponse.Token,
            userName = tokenResponse.UserName
        });
    }
    
}