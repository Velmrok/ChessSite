using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Extensions;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Text.Json;

namespace backend.Services;

public class GameRepository : IGameRepository
{
    private readonly IDatabase _db;
    private readonly AppDbContext _dbContext;

    private static string GameKey(string gameId) => $"games:{gameId}";
    private static string MovesKey(string gameId) => $"games:{gameId}:moves";
    private static string MessagesKey(string gameId) => $"games:{gameId}:messages";
    private static string UserActiveGameKey(string userId) => $"users:{userId}:activeGame";
    private static string UserAiGameKey(string userId) => $"users:{userId}:aiGame";
    private const string ActiveGamesSet = "activeGames";

    private const string MakeMoveScript = @"
        if redis.call('EXISTS', KEYS[1]) == 0 then
            return 'GAME_NOT_FOUND'
        end
        local whiteId = redis.call('HGET', KEYS[1], 'whiteplayerid')
        local blackId = redis.call('HGET', KEYS[1], 'blackplayerid')
        local isWhiteTurn = redis.call('HGET', KEYS[1], 'iswhiteturn')
        if ARGV[1] ~= whiteId and ARGV[1] ~= blackId then
            return 'NOT_IN_GAME'
        end
        local expected = (isWhiteTurn == 'True') and whiteId or blackId
        if ARGV[1] ~= expected then
            return 'NOT_YOUR_TURN'
        end
        redis.call('RPUSH', KEYS[2], ARGV[2])
        redis.call('HSET', KEYS[1],
            'iswhiteturn', ARGV[3],
            'currentwhitetime', ARGV[4],
            'currentblacktime', ARGV[5])
        return 'OK'
    ";

    private static readonly Dictionary<GamesSortBy, Expression<Func<Game, object>>> SortSelectors = new()
    {
        { GamesSortBy.FinishedAt, g => g.FinishedAt ?? DateTime.MaxValue },
        { GamesSortBy.Time, g => g.Time },
        { GamesSortBy.Nickname, g => g.WhitePlayer.Nickname },
    };

    public GameRepository(IConnectionMultiplexer redis, AppDbContext dbContext)
    {
        _db = redis.GetDatabase();
        _dbContext = dbContext;
    }

    public async Task<GameActive?> GetActiveGameAsync(string gameId)
    {
        var data = await _db.HashGetAllAsync(GameKey(gameId));
        if (data.Length == 0)
            return null;
        return data.FromHashEntries<GameActive>();
    }

    public async Task<List<MoveInfo>> GetMovesByGameIdAsync(string gameId)
    {
        var movesData = await _db.ListRangeAsync(MovesKey(gameId));
        return movesData
            .Select(m => JsonSerializer.Deserialize<MoveInfo>(m!))
            .OfType<MoveInfo>()
            .ToList();
    }

    public async Task<bool> IsActiveGameAsync(string gameId)
    {
        return await _db.SetContainsAsync(ActiveGamesSet, gameId);
    }

    public async Task<bool> IsInGameAsync(Guid userId)
    {
        return await _db.KeyExistsAsync(UserActiveGameKey(userId.ToString()));
    }

    public async Task<string?> GetActiveGameIdForUserAsync(Guid userId)
    {
        var value = await _db.StringGetAsync(UserActiveGameKey(userId.ToString()));
        return value.IsNull ? null : value.ToString();
    }

    public async Task<bool> CreateActiveGameAsync(GameActive game, string whitePlayerId, string blackPlayerId, bool setUserActiveKeys)
    {
        var tran = _db.CreateTransaction();
        _ = tran.HashSetAsync(GameKey(game.Id), game.ToHashEntries());
        _ = tran.SetAddAsync(ActiveGamesSet, game.Id);

        if (setUserActiveKeys)
        {
            _ = tran.StringSetAsync(UserActiveGameKey(whitePlayerId), game.Id);
            _ = tran.StringSetAsync(UserActiveGameKey(blackPlayerId), game.Id);
        }

        return await tran.ExecuteAsync();
    }

    public async Task<ErrorOr<Success>> ExecuteMakeMoveScriptAsync(
        string gameId, string userId, string serializedMove,
        bool newIsWhiteTurn, int newWhiteTime, int newBlackTime)
    {
        var luaResult = (string?)await _db.ScriptEvaluateAsync(
            MakeMoveScript,
            [GameKey(gameId), MovesKey(gameId)],
            [
                userId,
                serializedMove,
                newIsWhiteTurn ? "True" : "False",
                newWhiteTime.ToString(),
                newBlackTime.ToString()
            ]);
        switch (luaResult)
        {
            case "GAME_NOT_FOUND": return Error.NotFound("gameNotFound");
            case "NOT_IN_GAME": return Error.Failure("userNotInGame");
            case "NOT_YOUR_TURN": return Error.Failure("notUsersTurn");
            case "OK": break;
            default: return Error.Failure("unexpectedRedisError");
        }
        return new Success();
    }

    public async Task CleanupActiveGameAsync(string gameId, string whitePlayerId, string blackPlayerId)
    {
        var tran = _db.CreateTransaction();
        _ = tran.SetRemoveAsync(ActiveGamesSet, gameId);
        _ = tran.KeyDeleteAsync(GameKey(gameId));
        _ = tran.KeyDeleteAsync(MovesKey(gameId));
        _ = tran.KeyDeleteAsync(MessagesKey(gameId));
        _ = tran.KeyDeleteAsync(UserActiveGameKey(whitePlayerId));
        _ = tran.KeyDeleteAsync(UserActiveGameKey(blackPlayerId));
        await tran.ExecuteAsync();
    }

    public async Task SaveFinishedGameAsync(Game game)
    {
        using var tran = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();
            await tran.CommitAsync();
        }
        catch
        {
            await tran.RollbackAsync();
            throw;
        }
    }

    public async Task<Game?> GetFinishedGameByIdAsync(string gameId)
    {
        return await _dbContext.Games
            .Include(g => g.WhitePlayer)
            .Include(g => g.BlackPlayer)
            .Include(g => g.Winner)
            .FirstOrDefaultAsync(g => g.Id.ToString() == gameId);
    }

    public async Task<(List<Game> Games, int TotalPages)> GetFinishedGamesPagedAsync(GetGamesQuery query)
    {
        var search = query.Query ?? "";
        var gamesQuery = _dbContext.Games
            .Where(g => g.WhitePlayer.Nickname.Contains(search) || g.BlackPlayer.Nickname.Contains(search))
            .Where(g => query.GameType == null || g.Type == query.GameType)
            .Where(g => query.GameStatus == null || g.Status == query.GameStatus);

        var totalCount = await gamesQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

        var games = await gamesQuery
            .SortBy(SortSelectors[query.SortBy ?? GamesSortBy.FinishedAt], query.SortDescending)
            .Include(g => g.WhitePlayer)
            .Include(g => g.BlackPlayer)
            .Include(g => g.Winner)
            .Skip((query.PageNumber - 1) * query.Limit)
            .Take(query.Limit)
            .ToListAsync();

        return (games, totalPages);
    }

    public async Task RemoveStaleActiveGameAsync(string gameId)
    {
        await _db.SetRemoveAsync(ActiveGamesSet, gameId);
    }
    

    public async Task SetUserAiGameAsync(string userId, string gameId)
    {
        await _db.StringSetAsync(UserAiGameKey(userId), gameId);
    }

    public async Task<string?> GetUserAiGameAsync(string userId)
    {
        var value = await _db.StringGetAsync(UserAiGameKey(userId));
        return value.IsNull ? null : value.ToString();
    }

    public async Task RemoveUserAiGameAsync(string userId)
    {
        await _db.KeyDeleteAsync(UserAiGameKey(userId));
    }
}
