
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace backend.Services;
 
public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtGenerator _jwtGenerator;
    public AuthService(AppDbContext dbContext, IJwtGenerator jwtGenerator)
    {
        _dbContext = dbContext;
        _jwtGenerator = jwtGenerator;
    }
    public async Task<ErrorOr<AuthResponse>> RegisterAsync(RegisterRequest request)
    {

        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Nickname == request.Nickname || u.Email == request.Email || u.Login == request.Login);

        if (existingUser != null)
        {
            if (existingUser.Nickname == request.Nickname) return Error.Conflict("nicknameTaken", "Nickname is already taken.");
            if (existingUser.Email == request.Email) return Error.Conflict("emailTaken", "Email is already taken.");
            return Error.Conflict("loginTaken", "Login is already taken.");
        }

        var hasher = new PasswordHasher<object>();
        var passwordHash = hasher.HashPassword(null!, request.Password);
            var newUser = new User
            {
                Nickname = request.Nickname,
                Login = request.Login,
                Email = request.Email,
                PasswordHash = passwordHash,
                ProfileBio = "",
                ProfilePictureUrl = "",
            };
        await _dbContext.Users.AddAsync(newUser);

        var token = _jwtGenerator.GenerateToken(newUser);
        

        await _dbContext.SaveChangesAsync();
        return new AuthResponse(token);

    }
}