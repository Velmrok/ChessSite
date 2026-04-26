


using backend.Data;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;

namespace backend.Services;

public class HomeService : IHomeService
{
    private readonly AppDbContext   _context;
    public HomeService(AppDbContext context)
    {
        _context = context;    
    }

    public Task<ErrorOr<LeaderBoardResponse>> GetLeaderboardAsync()
    {
        var topRapidPlayers = _context.Users.OrderByDescending(u => u.Rating.Rapid)
        .Take(9)
        .Select(u => u.ToUserSummary()).ToList();

        var topBlitzPlayers = _context.Users.OrderByDescending(u => u.Rating.Blitz)
        .Take(9)
        .Select(u => u.ToUserSummary()).ToList();

        var topBulletPlayers = _context.Users.OrderByDescending(u => u.Rating.Bullet)
        .Take(9)
        .Select(u => u.ToUserSummary()).ToList();

        return Task.FromResult<ErrorOr<LeaderBoardResponse>>(new LeaderBoardResponse(
            topRapidPlayers,
            topBlitzPlayers,
            topBulletPlayers
        ));
    }
}