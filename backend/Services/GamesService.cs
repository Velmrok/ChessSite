using backend.Data;
using backend.DTO.Games;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPresenceService _presenceService;

        public GamesService(AppDbContext dbContext, IPresenceService presenceService)
        {
            _dbContext = dbContext;
            _presenceService = presenceService;
        }

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            var games = await _dbContext.Games
                .Where(g => g.WhitePlayer.Nickname.Contains(query.Query ?? "") || g.BlackPlayer.Nickname.Contains(query.Query ?? ""))
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .ToListAsync();

            var onlineIds = await _presenceService.GetOnlineIdsAsync([.. games.Select(g => g.WhitePlayerId), .. games.Select(g => g.BlackPlayerId)]);

            var totalCount = games.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

            var response = new GamesResponse
            (
                Games: [.. games.Select(g => g.MapToGamesResponse(
                    winnerNickname: g.Winner?.Nickname,
                    isWhitePlayerOnline: onlineIds.Contains(g.WhitePlayerId),
                    isBlackPlayerOnline: onlineIds.Contains(g.BlackPlayerId)))],

                TotalPages: totalPages
            );

            return response;

        }
    }
}