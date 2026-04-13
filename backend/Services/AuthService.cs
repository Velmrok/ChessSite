
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<User>> RegisterAsync(RegisterRequest request)
    {
        var hasher = new PasswordHasher<object>();
        var passwordHash = hasher.HashPassword(null, request.Password);
        await _dbContext.Users.AddAsync(new User
        {
            Nickname = request.Nickname,
            Login = request.Login,
            Email = request.Email,
            PasswordHash = passwordHash,
            ProfileBio = "",
            ProfilePictureUrl = "",
        });
        await _dbContext.SaveChangesAsync();
        return await _dbContext.Users.ToListAsync();
    }
}