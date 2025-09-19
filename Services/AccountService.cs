using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Exceptions;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;
public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
    TokenResponseDto GenerateToken(LoginDto dto);
}
public class AccountService : IAccountService
{
    private readonly RentalDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AccountService(RentalDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        var newUser = _mapper.Map<User>(dto);
        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.Password = hashedPassword;
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
    }

    
    public TokenResponseDto GenerateToken(LoginDto dto)
    {
        var user = _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefault(e => e.Email == dto.Email);
        if (user == null)
        {
            throw new BadRequestException("Podano zły email lub hasło");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Podano zły email lub hasło");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:ExpireDays"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims,
            expires: expirationDate,
            signingCredentials: cred
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        string generatedToken = tokenHandler.WriteToken(token);

        return new TokenResponseDto
        {
            Token = generatedToken,
            UserName = $"{user.FirstName} {user.LastName}"
        };
    }


}