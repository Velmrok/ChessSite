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

        private Dictionary<GamesSortBy, Expression<Func<Game, object>>> SortSelectors = new()
        {
            { GamesSortBy.FinishedAt, g => g.FinishedAt ?? DateTime.MaxValue },
            { GamesSortBy.Time, g => g.Time },
            { GamesSortBy.Nickname, g => g.WhitePlayer.Nickname },

        };

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            var search = query.Query ?? "";
            var gamesQuerry = _dbContext.Games
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

        public async Task<ErrorOr<string>> CreateGameAsync(string user1Id, string user2Id, int time, int increment)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == user1Id);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == user2Id);
            if (user1 == null || user2 == null)
                return Error.NotFound("userNotFound");
            var id = Guid.NewGuid();
            var game = new Game
            {
                Id = id,
                WhitePlayerId = user1.Id,
                BlackPlayerId = user2.Id,
                Time = time,
                Increment = increment,
                Type = time <= 3 ? GameType.Bullet : time <= 5 ? GameType.Blitz : GameType.Rapid
            };
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();

            return game.Id.ToString();
        }

        public async Task<ErrorOr<GameResponse>> GetGameByIdAsync(string gameId)
        {
            var game = await _dbContext.Games
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id.ToString() == gameId);

            if (game == null)
                return Error.NotFound("gameNotFound");

            return game.MapToGameResponse();
        }
    }
}