


using backend.Data;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class HomeService : IHomeService
{
    private readonly AppDbContext   _dbcontext;
    private readonly IPresenceService _presenceService;
    public HomeService(AppDbContext context, IPresenceService presenceService)
    {
        _dbcontext = context;    
        _presenceService = presenceService; 
    }

    public async Task<ErrorOr<LeaderBoardResponse>> GetLeaderboardAsync()
    {
        var topRapid = await _dbcontext.Users
            .OrderByDescending(u => u.Rating.Rapid)
            .Take(9)
            .ToListAsync();

        var topBlitz = await _dbcontext.Users
            .OrderByDescending(u => u.Rating.Blitz)
            .Take(9)
            .ToListAsync();

        var topBullet = await _dbcontext.Users
            .OrderByDescending(u => u.Rating.Bullet)
            .Take(9)
            .ToListAsync();

        var allUserIds = topRapid.Concat(topBlitz).Concat(topBullet)
            .Select(u => u.Id)
            .Distinct()
            .ToList();

        var onlineStatuses = await _presenceService.GetOnlineIdsAsync(allUserIds);

        var topRapidPlayers = topRapid.Select(u => new UserSummary
        {
            Nickname = u.Nickname,
            ProfilePictureUrl = u.ProfilePictureUrl,
            Rating = u.Rating.Rapid,
            IsOnline = onlineStatuses.Contains(u.Id)
        }).ToList();

        var topBlitzPlayers = topBlitz.Select(u => new UserSummary
        {
            Nickname = u.Nickname,
            ProfilePictureUrl = u.ProfilePictureUrl,
            Rating = u.Rating.Blitz,
            IsOnline = onlineStatuses.Contains(u.Id)
        }).ToList();

        var topBulletPlayers = topBullet.Select(u => new UserSummary
        {
            Nickname = u.Nickname,
            ProfilePictureUrl = u.ProfilePictureUrl,
            Rating = u.Rating.Bullet,
            IsOnline = onlineStatuses.Contains(u.Id)
        }).ToList();

        return new LeaderBoardResponse(
            topRapidPlayers,
            topBlitzPlayers,
            topBulletPlayers
        );
    }
}