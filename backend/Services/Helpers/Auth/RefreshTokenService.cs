

using backend.Data;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace backend.Services.Helpers.Auth;
public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _dbContext;

    public RefreshTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> CreateRefreshTokenAsync(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };
        await RemoveRefreshTokenAsync(refreshToken);

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }
    public async Task<bool> DoesRefreshTokenExistAsync(string refreshToken)
    {
        return await _dbContext.RefreshTokens.AnyAsync(rt => rt.Token == refreshToken);
    }
    public async Task RemoveRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (refreshTokenEntity != null)
        {
            _dbContext.RefreshTokens.Remove(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}