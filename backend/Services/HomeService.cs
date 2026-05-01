


using backend.Data;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.Enums;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class HomeService : IHomeService
{

    private readonly IPresenceService _presenceService;
    private readonly AppDbContext _dbContext;
    public HomeService(IPresenceService presenceService, AppDbContext dbContext)
    {
        _presenceService = presenceService; 
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LeaderBoardResponse>> GetLeaderboardAsync()
    {
        var topRapid = await _dbContext.Users
            .OrderByDescending(u => u.Rating.Rapid)
            .Take(9)
            .ToListAsync();

        var topBlitz = await _dbContext.Users
            .OrderByDescending(u => u.Rating.Blitz)
            .Take(9)
            .ToListAsync();

        var topBullet = await _dbContext.Users
            .OrderByDescending(u => u.Rating.Bullet)
            .Take(9)
            .ToListAsync();

        var allUserIds = topRapid.Concat(topBlitz).Concat(topBullet)
            .Select(u => u.Id)
            .Distinct()
            .ToList();

        var onlineStatuses = await _presenceService.GetOnlineIdsAsync(allUserIds);

        var topRapidPlayers = topRapid.Select(u => u.ToUserLeaderboardSummary(GameType.Rapid)).ToList();

        var topBlitzPlayers = topBlitz.Select(u => u.ToUserLeaderboardSummary(GameType.Blitz)).ToList();

        var topBulletPlayers = topBullet.Select(u => u.ToUserLeaderboardSummary(GameType.Bullet)).ToList();

        return new LeaderBoardResponse(
            topRapidPlayers,
            topBlitzPlayers,
            topBulletPlayers
        );
    }

    public async Task<ErrorOr<Success>> JoinHomeGroupAsync(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return Error.Unauthorized("invalidToken", "Token is invalid.");
            
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == guid);

        if (user == null)
            return Error.Unauthorized("invalidToken", "Token is invalid.");
        await _presenceService.SetOnlineAsync(guid);
        return new Success();
    }
}