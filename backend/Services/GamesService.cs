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
using Chess;
using Microsoft.AspNetCore.SignalR;
using backend.Hubs;
using backend.DTO.Common;
namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly IHubContext<MainHub> _hubContext;
        private readonly AppDbContext _dbContext;
        private readonly IDatabase _db;
        private readonly IGameTimerService _gameTimerService;
        private const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private static string GameKey(string gameId) => $"games:{gameId}";
        private static string MovesKey(string gameId) => $"games:{gameId}:moves";
        private static string MessagesKey(string gameId) => $"games:{gameId}:messages";
        private static string UserActiveGameKey(string userId) => $"users:{userId}:activeGame";
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

        public GamesService(AppDbContext dbContext,IConnectionMultiplexer redis, IHubContext<MainHub> hubContext, IGameTimerService gameTimerService)
        {
            _dbContext = dbContext;
            _db = redis.GetDatabase();
            _hubContext = hubContext;
            _gameTimerService = gameTimerService;
        }

        private Dictionary<GamesSortBy, Expression<Func<Game, object>>> SortSelectors = new()
        {
            { GamesSortBy.FinishedAt, g => g.FinishedAt ?? DateTime.MaxValue },
            { GamesSortBy.Time, g => g.Time },
            { GamesSortBy.Nickname, g => g.WhitePlayer.Nickname },
        };

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            // TODO ACTIVE GAMES
            var search = query.Query ?? "";
            var gamesQuery = _dbContext.Games
                .Where(g => g.WhitePlayer.Nickname.Contains(search) || g.BlackPlayer.Nickname.Contains(search))
                .Where(g => query.GameType == null || g.Type == query.GameType)
                .Where(g => query.GameStatus == null || g.Status == query.GameStatus);

            var totalCount = await gamesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

            var pagedGames = await gamesQuery
                .SortBy(SortSelectors[query.SortBy ?? GamesSortBy.FinishedAt], query.SortDescending)
                .Include(g => g.WhitePlayer)
                .Include(g => g.BlackPlayer)
                .Include(g => g.Winner)
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            var response = new GamesResponse(
                Games: [.. pagedGames.Select(g => g.MapToGamesResponse(winnerNickname: g.Winner?.Nickname))],
                TotalPages: totalPages
            );

            return response;
        }

        public async Task<ErrorOr<GameResponse>> GetGameByIdAsync(string gameId)
        {
            var isActive = await _db.SetContainsAsync(ActiveGamesSet, gameId);
            if (isActive)
            {
                var gameActiveData = await _db.HashGetAllAsync(GameKey(gameId));
                if (gameActiveData.Length == 0)
                {
                    await _db.SetRemoveAsync(ActiveGamesSet, gameId);
                }
                else
                {
                    var gameActive = gameActiveData.FromHashEntries<GameActive>();
                    var moves = await GetMovesByGameIdAsync(gameId);
                    List<MessageInfo> messages = []; 
                    return gameActive.MapToGameResponse(moves, messages);
                }
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
            return await _db.KeyExistsAsync(UserActiveGameKey(userId.ToString()));
        }

        public async Task<ErrorOr<string>> CreateGameAsync(string user1Id, string user2Id, int time, int increment)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == user1Id);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == user2Id);
            if (user1 == null || user2 == null)
                return Error.NotFound("userNotFound");
            if (await IsInGameAsync(user1.Id) || await IsInGameAsync(user2.Id))
                return Error.Failure("oneOrBothUsersInGame");

            int rand = Random.Shared.Next(0, 2);
            var whitePlayer = rand == 0 ? user1 : user2;
            var blackPlayer = rand == 0 ? user2 : user1;

            var id = Guid.NewGuid();

            var gameActive = new GameActive
            {
                Id = id.ToString(),
                WhitePlayerId = whitePlayer.Id.ToString(),
                BlackPlayerId = blackPlayer.Id.ToString(),
                WhitePlayerNickname = whitePlayer.Nickname,
                BlackPlayerNickname = blackPlayer.Nickname,
                WhitePlayerProfilePictureUrl = whitePlayer.ProfilePictureUrl,
                BlackPlayerProfilePictureUrl = blackPlayer.ProfilePictureUrl,
                WhitePlayerRating = whitePlayer.GetRatingByTime(time),
                BlackPlayerRating = blackPlayer.GetRatingByTime(time),
                Time = time,
                Increment = increment,
                GameType = time <= 3 ? GameType.Bullet : time <= 5 ? GameType.Blitz : GameType.Rapid,
                IsWhiteTurn = true,
                CurrentWhiteTime = time * 60_000,
                CurrentBlackTime = time * 60_000
            };

            var tran = _db.CreateTransaction();
            _ = tran.HashSetAsync(GameKey(id.ToString()), gameActive.ToHashEntries());
            _ = tran.SetAddAsync(ActiveGamesSet, id.ToString());
            _ = tran.StringSetAsync(UserActiveGameKey(whitePlayer.Id.ToString()), id.ToString());
            _ = tran.StringSetAsync(UserActiveGameKey(blackPlayer.Id.ToString()), id.ToString());

            if (!await tran.ExecuteAsync())
                return Error.Failure("redisTransactionFailed");

            _gameTimerService.ScheduleTimeout(
                id.ToString(),
                whitePlayer.Id.ToString(),
                30_000);

            return id.ToString();
        }

        public async Task<ErrorOr<EmptyResponse>> MakeMoveAsync(string? userId, SignalRRequest<MakeMoveRequest> request)
        {
            if (userId == null) return Error.Unauthorized("userNotAuthenticated");

            var gameId = request.Payload!.GameId;

            var gameActiveData = await _db.HashGetAllAsync(GameKey(gameId));
            if (gameActiveData.Length == 0)
                return Error.NotFound("gameNotFound");

            var gameActive = gameActiveData.FromHashEntries<GameActive>();

            if (gameActive.WhitePlayerId != userId && gameActive.BlackPlayerId != userId)
                return Error.Failure("userNotInGame");

            var currentTurnPlayerId = gameActive.IsWhiteTurn ? gameActive.WhitePlayerId : gameActive.BlackPlayerId;
            if (currentTurnPlayerId != userId)
                return Error.Failure("notUsersTurn");

            var moves = await GetMovesByGameIdAsync(gameId);
            var currentFen = moves.Count == 0 ? DefaultFen : moves.Last().Fen;

            var board = ChessBoard.LoadFromFen(currentFen);
            var move = request.Payload.San;

            if (!board.IsValidMove(move))
                return Error.Failure("invalidMove");

            board.Move(move);
            var newFen = board.ToFen();
            var now = DateTime.UtcNow;


            var lastMoveTimestampStr = moves.LastOrDefault() != null ? moves.Last().Timestamp.ToString("O") : null;
            var deltaTime = lastMoveTimestampStr != null ? (int)(now - DateTime.Parse(lastMoveTimestampStr)).TotalMilliseconds : 0;

            int newWhiteTime = gameActive.CurrentWhiteTime;
            int newBlackTime = gameActive.CurrentBlackTime;

            if (gameActive.IsWhiteTurn)
                newWhiteTime -= deltaTime;
            else
                newBlackTime -= deltaTime;

            var absoluteTime = gameActive.IsWhiteTurn ? newWhiteTime : newBlackTime;

            if (absoluteTime < 0)
            {
                var winnerId = gameActive.IsWhiteTurn ? gameActive.BlackPlayerId : gameActive.WhitePlayerId;
                return await EndGameInternal(gameId, gameActive, "timeout", winnerId);
            }

            if (gameActive.IsWhiteTurn)
                newWhiteTime += gameActive.Increment * 1000;
            else
                newBlackTime += gameActive.Increment * 1000;

            var moveInfo = new MoveInfo(move, newFen, deltaTime, absoluteTime, now);
            var serializedMove = JsonSerializer.Serialize(moveInfo);
            var newIsWhiteTurn = !gameActive.IsWhiteTurn;

            // lua used to eliminate race condition
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

            await _hubContext.Clients.Group($"Game:{gameId}")
                .SendAsync("MoveMade", new SignalRResponse<MoveInfo>(
                    Type: "MoveMade",
                    CorrelationId: gameId,
                    Data: moveInfo
                ));
            _gameTimerService.ScheduleTimeout(
                gameId,
                newIsWhiteTurn ? gameActive.WhitePlayerId : gameActive.BlackPlayerId,
                gameActive.IsWhiteTurn ? newWhiteTime : newBlackTime);

            return new EmptyResponse();
        }

        public async Task<ErrorOr<EmptyResponse>> SurrenderGameAsync(string? userId, string gameId)
        {
            if (userId == null) return Error.Unauthorized("userNotAuthenticated");

            var gameActiveData = await _db.HashGetAllAsync(GameKey(gameId));
            if (gameActiveData.Length == 0)
                return Error.NotFound("gameNotFound");

            var gameActive = gameActiveData.FromHashEntries<GameActive>();

            if (gameActive.WhitePlayerId != userId && gameActive.BlackPlayerId != userId)
                return Error.Failure("userNotInGame");

            var winnerId = gameActive.WhitePlayerId == userId
                ? gameActive.BlackPlayerId
                : gameActive.WhitePlayerId;

            return await EndGameInternal(gameId, gameActive, "surrender", winnerId);
        }
        public async Task HandleTimeoutAsync(string gameId, string timedOutPlayerId)
        {
            var gameActiveData = await _db.HashGetAllAsync(GameKey(gameId));
            if (gameActiveData.Length == 0)
                return; 

            var gameActive = gameActiveData.FromHashEntries<GameActive>();

            var currentTurnPlayerId = gameActive.IsWhiteTurn
                ? gameActive.WhitePlayerId
                : gameActive.BlackPlayerId;

            if (currentTurnPlayerId != timedOutPlayerId)
                return; 

            var winnerId = gameActive.WhitePlayerId == timedOutPlayerId
                ? gameActive.BlackPlayerId
                : gameActive.WhitePlayerId;

            await EndGameInternal(gameId, gameActive, "timeout", winnerId);
        }

        // ==================== HELPERS ====================

        private async Task<List<MoveInfo>> GetMovesByGameIdAsync(string gameId)
        {
            var movesData = await _db.ListRangeAsync(MovesKey(gameId));
            return movesData
                .Select(m => JsonSerializer.Deserialize<MoveInfo>(m!))
                .OfType<MoveInfo>()
                .ToList();
        }
        private async Task<ErrorOr<EmptyResponse>> EndGameInternal(
            string gameId, GameActive gameActive, string reason, string? winnerId)
        {
            _gameTimerService.RemoveGame(gameId);
            var moves = await GetMovesByGameIdAsync(gameId);

           Guid? winnerGuid = Guid.TryParse(winnerId, out var g) ? g : null;
            
            var whiteGuid = Guid.Parse(gameActive.WhitePlayerId);
            var blackGuid = Guid.Parse(gameActive.BlackPlayerId);

            var users = await _dbContext.Users
                .Where(u => u.Id == winnerGuid || u.Id == whiteGuid || u.Id == blackGuid)
                .ToListAsync();

            var winner = users.FirstOrDefault(u => u.Id == winnerGuid);
           
            var game = new Game
            {
                Id = Guid.Parse(gameId),
                WhitePlayerId = whiteGuid,
                BlackPlayerId = blackGuid,
                WinnerId = winnerGuid,
                Time = gameActive.Time,
                Increment = gameActive.Increment,
                Type = gameActive.GameType,
                Status = GameStatus.Finished,
                FinishedAt = DateTime.UtcNow,
                Moves = moves
            };

            using (var tran = await _dbContext.Database.BeginTransactionAsync())
            {
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

            var redisTran = _db.CreateTransaction();
            _ = redisTran.SetRemoveAsync(ActiveGamesSet, gameId);
            _ = redisTran.KeyDeleteAsync(GameKey(gameId));
            _ = redisTran.KeyDeleteAsync(MovesKey(gameId));
            _ = redisTran.KeyDeleteAsync(MessagesKey(gameId));
            _ = redisTran.KeyDeleteAsync(UserActiveGameKey(gameActive.WhitePlayerId));
            _ = redisTran.KeyDeleteAsync(UserActiveGameKey(gameActive.BlackPlayerId));

            var redisOk = await redisTran.ExecuteAsync();
            
            await _hubContext.Clients.Group($"Game:{gameId}").SendAsync("GameEnded",
                new SignalRResponse<GameEndedResponse>(
                    Type: "GameEnded",
                    CorrelationId: gameId,
                    Data: new GameEndedResponse(gameId, winner is not null ? winner.Nickname : null , reason)
                ));

            return new EmptyResponse();
        }
    }
}