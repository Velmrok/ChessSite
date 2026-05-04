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
using StackExchange.Redis;
using System.Text.Json;
namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPresenceService _presenceService;
        private readonly IDatabase _db;

        public GamesService(AppDbContext dbContext, IPresenceService presenceService, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _presenceService = presenceService;
            _db = redis.GetDatabase();
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
            int rand = Random.Shared.Next(0, 2);
            var whitePlayer = rand == 0 ? user1 : user2;
            var blackPlayer = rand == 0 ? user2 : user1;

            var id = Guid.NewGuid();
            var game = new Game
            {
                Id = id,
                WhitePlayerId = whitePlayer.Id,
                BlackPlayerId = blackPlayer.Id,
                Time = time,
                Increment = increment,
                Type = time <= 3 ? GameType.Bullet : time <= 5 ? GameType.Blitz : GameType.Rapid
            };
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();

            var gameActive = game.MapToGameActive();
            
            await _db.HashSetAsync("games:"+id, gameActive.ToHashEntries());
            await _db.SetAddAsync("activeGames", id.ToString());
           

            return game.Id.ToString();
        }

        public async Task<ErrorOr<GameResponse>> GetGameByIdAsync(string gameId)
        {
            var isActive = await _db.SetContainsAsync("activeGames", gameId);
            if (isActive)
            {
                var gameActiveData = await _db.HashGetAllAsync("games:" + gameId);
                var gameActive = gameActiveData.FromHashEntries<GameActive>();
                var movesData = await _db.ListRangeAsync($"games:{gameId}:moves");
                var moves = (await _db.ListRangeAsync($"game:{gameId}:moves"))
                    .Select(m => JsonSerializer.Deserialize<MoveInfo>(m))
                    .OfType<MoveInfo>()
                    .ToList();
                List<MessageInfo> messages = [];
                return gameActive.MapToGameResponse(moves, messages);
            }
            var game = await _dbContext.Games
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id.ToString() == gameId);

            if (game == null)
                return Error.NotFound("gameNotFound");

            return game.MapToGameResponse();
        }

        public async Task<bool> IsInGameAsync(Guid userId)
        {
    
            return await _dbContext
            .Games.AnyAsync(g => (g.WhitePlayerId == userId || g.BlackPlayerId == userId) && g
            .Status == GameStatus.Active);
        }
    }
}