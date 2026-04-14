
using backend.Data;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    public UsersService(AppDbContext context)
    {
        _context = context;
    }
    public Task<List<User>> GetAllUsersAsync()
    {
        var result = _context.Users.ToListAsync();
        return result;
    }
}
