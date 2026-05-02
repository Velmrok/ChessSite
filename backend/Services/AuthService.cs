using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
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
    private readonly IStorageService _storageService;
    private readonly IPresenceService _presenceService;
    private readonly IQueueService _queueService;

    public AuthService(
        AppDbContext dbContext,
        IJwtGenerator jwtGenerator,
        IPasswordHasher<User> passwordHasher,
        ICacheInvalidationService cacheInvalidation,
        IRefreshTokenService refreshTokenService,
        IStorageService storageService,
        IPresenceService presenceService,
        IQueueService queueService)
    {
        _dbContext = dbContext;
        _jwtGenerator = jwtGenerator;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
        _cacheInvalidation = cacheInvalidation;
        _storageService = storageService;
        _presenceService = presenceService;
        _queueService = queueService;
    }

    public async Task<ErrorOr<AuthResult>> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Nickname = request.Nickname.Trim(),
            Login = request.Login.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = _passwordHasher.HashPassword(null!, request.Password),
            ProfileBio = "",
            ProfilePictureUrl = _storageService.GetAvatarUrl("default.webp"),
        };

        _dbContext.Users.Add(user);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return await MapConflictErrorAsync(request);
        }

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);
        await _presenceService.SetOnlineAsync(user.Id);
        await _dbContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateUsersCache();

        return BuildAuthResult(user, accessToken, refreshToken, []);
    }

    public async Task<ErrorOr<AuthResult>> LoginAsync(LoginRequest request)
    {
        var login = request.Login.Trim();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

        if (user is null)
            return Error.Unauthorized("invalidLoginOrPassword", "Invalid login or password.");

        var result = _passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Error.Unauthorized("invalidLoginOrPassword", "Invalid login or password.");

        var (accessToken, refreshToken) = await GenerateTokensAsync(user);
        await _dbContext.SaveChangesAsync();

        var friendNicknames = await GetFriendNicknamesAsync(user.Id);
        await _presenceService.SetOnlineAsync(user.Id);

        return BuildAuthResult(user, accessToken, refreshToken, friendNicknames);
    }

    public async Task<ErrorOr<AuthResult>> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is missing.");

        var tokenEntity = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity is null || tokenEntity.ExpiresAt < DateTime.UtcNow || tokenEntity.IsRevoked)
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is invalid or has expired.");

        var user = tokenEntity.User;

        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);
        await _dbContext.SaveChangesAsync();

        var friendNicknames = await GetFriendNicknamesAsync(user.Id);
        await _presenceService.SetOnlineAsync(user.Id);

        return BuildAuthResult(user, newAccessToken, newRefreshToken, friendNicknames);
    }

    public async Task<ErrorOr<Success>> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return Error.Unauthorized("invalidRefreshToken", "Refresh token is missing.");

        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<ErrorOr<GetMeResponse>> GetMeAsync(string sub)
    {
        if (!Guid.TryParse(sub, out var userId))
            return Error.Unauthorized("invalidAccessToken", "Access token is invalid.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return Error.NotFound("userNotFound", "User account no longer exists.");

        var friendNicknames = await GetFriendNicknamesAsync(user.Id);
        await _presenceService.SetOnlineAsync(user.Id);

        var queueData = _queueService.GetUserQueueData(user.Id.ToString());
        return user.ToGetMeResponse(friendNicknames, queueData);
    }

    // ── Private helpers ──────────────────────────────────────────

    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = _jwtGenerator.GenerateToken(user);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);
        return (accessToken, refreshToken);
    }

    private async Task<List<string>> GetFriendNicknamesAsync(Guid userId)
    {
        return await _dbContext.Friendships
            .Where(f => f.UserId == userId)
            .Select(f => f.Friend.Nickname)
            .ToListAsync();
    }

    private AuthResult BuildAuthResult(
        User user, string accessToken, string refreshToken, List<string> friendNicknames)
    {
        var queueData = _queueService.GetUserQueueData(user.Id.ToString());
        return new AuthResult(accessToken, refreshToken, user.ToGetMeResponse(friendNicknames, queueData));
    }

    private async Task<Error> MapConflictErrorAsync(RegisterRequest request)
    {
        _dbContext.ChangeTracker.Clear();

        var conflict = await _dbContext.Users
            .Where(u => u.Nickname == request.Nickname ||
                        u.Email == request.Email ||
                        u.Login == request.Login)
            .Select(u => new { u.Nickname, u.Email, u.Login })
            .FirstOrDefaultAsync();

        if (conflict is null)
            return Error.Failure("registrationFailed", "Registration failed due to a database error.");

        if (conflict.Nickname == request.Nickname)
            return Error.Conflict("nicknameTaken", "Nickname is already taken.");
        if (conflict.Email == request.Email)
            return Error.Conflict("emailTaken", "Email is already taken.");
        return Error.Conflict("loginTaken", "Login is already taken.");
    }
}