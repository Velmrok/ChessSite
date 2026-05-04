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
namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly IHubContext<MainHub> _hubContext;
        private readonly AppDbContext _dbContext;
        private readonly IPresenceService _presenceService;
        private readonly IDatabase _db;
        private const string _defaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public GamesService(AppDbContext dbContext, IPresenceService presenceService, IConnectionMultiplexer redis, IHubContext<MainHub> hubContext)
        {
            _dbContext = dbContext;
            _presenceService = presenceService;
            _db = redis.GetDatabase();
            _hubContext = hubContext;
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
        
                var moves = GetMovesByGameId(gameId);
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
        public async Task<ErrorOr<MoveInfo>> MakeMoveAsync(string? userId, MakeMoveRequest request)
        {
            var gameId = request.GameId;
            var isActive = await _db.SetContainsAsync("activeGames", gameId);
            if (!isActive)
                return Error.NotFound("gameNotFound");

            var gameActiveData = await _db.HashGetAllAsync("games:" + gameId);
            var gameActive = gameActiveData.FromHashEntries<GameActive>();

            if (gameActive.WhitePlayerId != userId && gameActive.BlackPlayerId != userId)
                return Error.Failure("userNotInGame");

            var currentTurnPlayerId = gameActive.IsWhiteTurn ? gameActive.WhitePlayerId : gameActive.BlackPlayerId;
            if (currentTurnPlayerId != userId)
                return Error.Failure("notUsersTurn");
            
            var moves = GetMovesByGameId(gameId);
            
            var currentFen = moves.Count == 0 ? _defaultFen : moves.Last().Fen;
           
            var board = ChessBoard.LoadFromFen(currentFen);

            var move = request.San;

            var isValidMove = board.IsValidMove(move);
            if (!isValidMove)
                return Error.Failure("invalidMove");
            board.Move(move);
            var newFen = board.ToFen();
            var now = DateTime.UtcNow;

            var currentTimestamp = moves.Count == 0 ? now : moves.Last().Timestamp;
            var deltaTime = (int)(now - currentTimestamp).TotalMilliseconds;

            if(gameActive.IsWhiteTurn)
                gameActive.CurrentWhiteTime -= deltaTime;
            else
                gameActive.CurrentBlackTime -= deltaTime;
            var absoluteTime = gameActive.IsWhiteTurn ? gameActive.CurrentWhiteTime : gameActive.CurrentBlackTime;

            if (absoluteTime < 0)
            {
                // todo endgame
                return Error.Failure("timeOut");
            }

            var moveInfo = new MoveInfo(move, newFen, deltaTime, absoluteTime, now);
            await _db.ListRightPushAsync($"games:{gameId}:moves", JsonSerializer.Serialize(moveInfo));

            gameActive.IsWhiteTurn = !gameActive.IsWhiteTurn;
            await _db.HashSetAsync("games:" + gameId, gameActive.ToHashEntries());
            
            return moveInfo;
        }

        // --------- helpers ---------
        private List<MoveInfo> GetMovesByGameId(string gameId)
        {
            var movesData = _db.ListRangeAsync($"games:{gameId}:moves").Result;
            var moves = movesData
                .Select(m => JsonSerializer.Deserialize<MoveInfo>(m))
                .OfType<MoveInfo>()
                .ToList();
            return moves;
        }
        private async Task<ErrorOr<Success>> EndGame(string gameId, string winnerId, string reason)
        {
            var gameActiveData = await _db.HashGetAllAsync("games:" + gameId);
            var gameActive = gameActiveData.FromHashEntries<GameActive>();
            

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id.ToString() == gameId);
            if (game == null)
                return Error.NotFound("gameNotFound");
            if(gameActive == null)
                return Error.Failure("gameNotActive");

            var moves = GetMovesByGameId(gameId);

            game.Status = GameStatus.Finished;
            game.FinishedAt = DateTime.UtcNow;
            game.Moves = moves;
            
            string? winnerNickname = null;
            if (winnerId != null)
            {
                var winner = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == winnerId);
                if (winner != null)
                {
                    game.WinnerId = winner.Id;
                    winnerNickname = winner.Nickname;
                }
                else
                {
                    return Error.NotFound("winnerNotFound");
                }
            }
            await _dbContext.SaveChangesAsync();

            await _db.SetRemoveAsync("activeGames", gameId);
            await _db.KeyDeleteAsync("games:" + gameId);
            await _db.KeyDeleteAsync($"games:{gameId}:moves");
            await _db.KeyDeleteAsync($"games:{gameId}:messages");

            await _hubContext.Clients.Group(gameId).SendAsync("GameEnded", new GameEndedResponse(
                GameId: gameId,
                Winner: winnerNickname,
                Reason: reason
            ));

            return new Success();
        }
    }
}