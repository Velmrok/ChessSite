
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using backend.Services.Helpers.Auth;
using backend.DTO.Users;
using backend.Services.Mappers;
using backend.Services.Results;

namespace backend.Services;
 
public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public AuthService(AppDbContext dbContext, IJwtGenerator jwtGenerator,
     IPasswordHasher<User> passwordHasher, ICacheInvalidationService cacheInvalidation,
     IRefreshTokenService refreshTokenService)
    {
        _dbContext = dbContext;
        _jwtGenerator = jwtGenerator;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
        _cacheInvalidation = cacheInvalidation;
    }
    public async Task<ErrorOr<AuthResult>> RegisterAsync(RegisterRequest request)
    {


        var passwordHash = _passwordHasher.HashPassword(null!, request.Password);
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

        


        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var conflictingUser = await _dbContext.Users
                .Where(u => u.Nickname == request.Nickname ||
                            u.Email == request.Email ||
                            u.Login == request.Login)
                .Select(u => new { u.Nickname, u.Email, u.Login })
                .FirstOrDefaultAsync();

            if (conflictingUser == null)
                return Error.Failure("registrationFailed", "Registration failed due to a database error.");

            if (conflictingUser.Nickname == request.Nickname) return Error.Conflict("nicknameTaken", "Nickname is already taken.");
            if (conflictingUser.Email == request.Email) return Error.Conflict("emailTaken", "Email is already taken.");
            return Error.Conflict("loginTaken", "Login is already taken.");
        }
        var accessToken = _jwtGenerator.GenerateToken(newUser);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(newUser);
        await _dbContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateUsersCache();
        return new AuthResult(accessToken, refreshToken, newUser.ToGetMeResponse());

    }
    public async Task<ErrorOr<AuthResult>> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Login == request.Login || u.Email == request.Login);

        if (user == null)
        {
            return Error.Unauthorized("invalidLoginOrPassword", "Invalid login or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Error.Unauthorized("invalidLoginOrPassword", "Invalid login or password.");
        }

        var accessToken = _jwtGenerator.GenerateToken(user);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);
        await _dbContext.SaveChangesAsync();
        
        return new AuthResult(accessToken, refreshToken, user.ToGetMeResponse());
    }
    public async Task<ErrorOr<AuthResult>> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is missing.");
        }

        var refreshTokenEntity = await _dbContext.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow || refreshTokenEntity.IsRevoked)
        {
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is invalid or has expired.");
        }

        var user = refreshTokenEntity.User;
        var newAccessToken = _jwtGenerator.GenerateToken(user);

        var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        await _dbContext.SaveChangesAsync();

        return new AuthResult(newAccessToken, newRefreshToken, user.ToGetMeResponse());
    }
    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
       if (string.IsNullOrEmpty(refreshToken))
        {
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is missing.");
        }
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
        return new Success();
    }

    public async Task<ErrorOr<GetMeResponse>> GetMeAsync(string sub)
    {
        if (string.IsNullOrEmpty(sub))
        {
            return Error.Unauthorized("invalidAccessToken", "Access token is invalid.");
        }
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == sub);
        if (userEntity == null)
            return Error.NotFound("userNotFound", "User account no longer exists.");
        return userEntity.ToGetMeResponse();
    }
}