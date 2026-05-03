using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using backend.Extensions;
using System.Linq.Expressions;
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

        private Dictionary<GamesSortBy,Expression<Func<Game, object>>> SortSelectors = new()
        {
            { GamesSortBy.FinishedAt, g => g.FinishedAt ?? DateTime.MaxValue },
            { GamesSortBy.Time, g => g.Time },
            { GamesSortBy.Nickname, g => g.WhitePlayer.Nickname },

        };

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            var search = query.Query ?? "";
            var gamesQuerry =  _dbContext.Games
                .Where(g => g.WhitePlayer.Nickname.Contains(search) || g.BlackPlayer.Nickname.Contains(search))
                .Where(g => query.GameType == null || g.Type == query.GameType)
                .Where(g => query.GameStatus == null || g.Status == query.GameStatus);
                
                

            var totalCount = await gamesQuerry.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

            var pagedGames = await gamesQuerry
                .SortBy(SortSelectors[query.SortBy ?? GamesSortBy.FinishedAt], query.SortDescending)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            var response = new GamesResponse
            (
                Games: [.. pagedGames.Select(g => g.MapToGamesResponse(winnerNickname: g.Winner?.Nickname))],
                TotalPages: totalPages
            );

            return response;

        }
    }
}