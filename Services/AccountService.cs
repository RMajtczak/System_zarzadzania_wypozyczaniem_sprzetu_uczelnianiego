using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;
public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
}
public class AccountService : IAccountService
{
    private readonly RentalDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountService(RentalDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        var newUser = _mapper.Map<User>(dto);
        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.Password = hashedPassword;
        _context.Users.Add(newUser);
        _context.SaveChanges();
        
    }
}