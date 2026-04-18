
using backend.Data;
using backend.DTO.Users;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    public UsersService(AppDbContext context)
    {
        _context = context;
    }
    public Task<List<UserResponse>> GetAllUsersAsync()
    {
        var result = _context.Users.Select(u => u.ToUserResponse()).ToListAsync();
        return result;
    }
}
