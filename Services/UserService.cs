using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> UpdateUserRoleAsync(int userId, int roleId);
}

public class UserService : IUserService
{
    private readonly RentalDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(RentalDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _dbContext.Users
            .Include(u => u.Role)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
    public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
    {
        var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (role == null) return false;

        user.Role = role;
        await _dbContext.SaveChangesAsync();
        return true;
    }
    
}