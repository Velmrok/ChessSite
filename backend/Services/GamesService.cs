using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using backend.DTO.Common;
using Chess;
using Microsoft.AspNetCore.SignalR;
using backend.Hubs;
using System.Text.Json;

namespace backend.Services
{
    public class GamesService : IGamesService
    {
        private readonly IHubContext<MainHub> _hubContext;
        private readonly AppDbContext _dbContext;
        private readonly IGameRepository _gameRepository;
        private readonly IGameTimerService _gameTimerService;
        private readonly IChessEngine _chessEngine;
        private readonly string _aiPlayerId;
        private const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public GamesService(
            AppDbContext dbContext,
            IHubContext<MainHub> hubContext,
            IGameTimerService gameTimerService,
            IGameRepository gameRepository,
            IChessEngine chessEngine,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _gameTimerService = gameTimerService;
            _gameRepository = gameRepository;
            _chessEngine = chessEngine;
            _aiPlayerId = configuration["SystemUsers:AiPlayer:Id"] ?? throw new Exception("AIPlayerId not configured"); 
        }

        public async Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query)
        {
            var (games, totalPages) = await _gameRepository.GetFinishedGamesPagedAsync(query);

            var response = new GamesResponse(
                Games: [.. games.Select(g => g.MapToGamesResponse(winnerNickname: g.Winner?.Nickname))],
                TotalPages: totalPages
            );

            return response;
        }

        public async Task<ErrorOr<GameResponse>> GetGameByIdAsync(string gameId)
        {
            if (await _gameRepository.IsActiveGameAsync(gameId))
            {
                var gameActive = await _gameRepository.GetActiveGameAsync(gameId);
                if (gameActive == null)
                {
                    await _gameRepository.RemoveStaleActiveGameAsync(gameId);
                }
                else
                {
                    var moves = await _gameRepository.GetMovesByGameIdAsync(gameId);
                    List<MessageInfo> messages = [];
                    return gameActive.MapToGameResponse(moves, messages);
                }
            }

            var game = await _gameRepository.GetFinishedGameByIdAsync(gameId);
            if (game == null)
                return Error.NotFound("gameNotFound");

            return game.MapToGameResponse();
        }

        public async Task<bool> IsInGameAsync(Guid userId)
        {
            return await _gameRepository.IsInGameAsync(userId);
        }

        public async Task<ErrorOr<string>> CreateGameAsync(string? user1Id, string user2Id, int time, int increment, int aiDifficulty = 1)
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
            bool isAiGame = _aiPlayerId == user1Id || _aiPlayerId == user2Id;

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
                CurrentBlackTime = time * 60_000,
                IsAiGame = isAiGame,
                AiDifficulty = aiDifficulty
            };

            

            if (!await _gameRepository
                .CreateActiveGameAsync(gameActive, whitePlayer.Id.ToString(), blackPlayer.Id.ToString(), setUserActiveKeys: !isAiGame))
                return Error.Failure("redisTransactionFailed");
            if(isAiGame)
            {
                var userId = whitePlayer.Id.ToString() == _aiPlayerId ? blackPlayer.Id.ToString() : whitePlayer.Id.ToString();
                await _gameRepository.SetUserAiGameAsync(userId, id.ToString());
            }
            else
            {
                await _gameRepository.RemoveUserAiGameAsync(whitePlayer.Id.ToString());
                await _gameRepository.RemoveUserAiGameAsync(blackPlayer.Id.ToString());
            }
            
            if (isAiGame && whitePlayer.Id.ToString() == _aiPlayerId)
            {
               _ =  MakeAIMoveAsync(gameActive);
            }

            _gameTimerService.ScheduleTimeout(
                id.ToString(),
                whitePlayer.Id.ToString(),
                30_000);

            return id.ToString();
        }

        public async Task<ErrorOr<EmptyResponse>> MakeMoveAsync(string? userId, SignalRRequest<MakeMovePayload> request)
        {
            if (userId == null) return Error.Unauthorized("userNotAuthenticated");

            var gameId = request.Payload!.GameId;

            var gameActive = await _gameRepository.GetActiveGameAsync(gameId);
            if (gameActive == null)
                return Error.NotFound("gameNotFound");

            if (gameActive.WhitePlayerId != userId && gameActive.BlackPlayerId != userId)
                return Error.Failure("userNotInGame");

            var currentTurnPlayerId = gameActive.IsWhiteTurn ? gameActive.WhitePlayerId : gameActive.BlackPlayerId;
            if (currentTurnPlayerId != userId)
                return Error.Failure("notUsersTurn");

            var moves = await _gameRepository.GetMovesByGameIdAsync(gameId);
            var currentFen = moves.Count == 0 ? DefaultFen : moves.Last().Fen;

            var board = ChessBoard.LoadFromFen(currentFen, AutoEndgameRules.All);

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

            var luaResult = await _gameRepository.ExecuteMakeMoveScriptAsync(
                gameId, userId, serializedMove, newIsWhiteTurn, newWhiteTime, newBlackTime);

            if (luaResult.IsError)
            {
                return luaResult.Errors.First();
            }

            await _hubContext.Clients.Group($"Game:{gameId}")
                .SendAsync("MoveMade", new SignalRResponse<MoveInfo>(
                    Type: "MoveMade",
                    CorrelationId: gameId,
                    Data: moveInfo
                ));

            if (board.IsEndGame)
            {
                await HandleEndGameAsync(gameId, gameActive, board.EndGame!);
                return new EmptyResponse();
            }
            
            _gameTimerService.ScheduleTimeout(
                gameId,
                newIsWhiteTurn ? gameActive.WhitePlayerId : gameActive.BlackPlayerId,
                gameActive.IsWhiteTurn ? newWhiteTime : newBlackTime);
            if (gameActive.IsAiGame)
            {
                await MakeAIMoveAsync(gameActive, newFen);
            }

            return new EmptyResponse();
        }

        public async Task<ErrorOr<EmptyResponse>> SurrenderGameAsync(string? userId, string gameId)
        {
            if (userId == null) return Error.Unauthorized("userNotAuthenticated");

            var gameActive = await _gameRepository.GetActiveGameAsync(gameId);
            if (gameActive == null)
                return Error.NotFound("gameNotFound");

            if (gameActive.WhitePlayerId != userId && gameActive.BlackPlayerId != userId)
                return Error.Failure("userNotInGame");

            var winnerId = gameActive.WhitePlayerId == userId
                ? gameActive.BlackPlayerId
                : gameActive.WhitePlayerId;

            return await EndGameInternal(gameId, gameActive, "surrender", winnerId);
        }

        public async Task HandleTimeoutAsync(string gameId, string timedOutPlayerId)
        {
            var gameActive = await _gameRepository.GetActiveGameAsync(gameId);
            if (gameActive == null)
                return;

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

        private async Task HandleEndGameAsync(string gameId, GameActive gameActive, EndGameInfo endGameInfo)
        {
            var winnerId = endGameInfo.WonSide?.AsChar switch
            {
                'w' => gameActive.WhitePlayerId,
                'b' => gameActive.BlackPlayerId,
                _ => null
            };
            var reason = endGameInfo.EndgameType switch
            {
                EndgameType.Checkmate => "checkmate",
                EndgameType.Stalemate => "stalemate",
                EndgameType.Repetition => "threefoldRepetition",
                EndgameType.FiftyMoveRule => "fiftyMoveRule",
                EndgameType.InsufficientMaterial => "insufficientMaterial",
                _ => "unknown"
            };
            await EndGameInternal(gameId, gameActive, reason, winnerId);
        }

        private async Task<ErrorOr<EmptyResponse>> EndGameInternal(
            string gameId, GameActive gameActive, string reason, string? winnerId)
        {
            _gameTimerService.RemoveGame(gameId);
            var moves = await _gameRepository.GetMovesByGameIdAsync(gameId);

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
            if(!gameActive.IsAiGame)
                await _gameRepository.SaveFinishedGameAsync(game);

            await _gameRepository.CleanupActiveGameAsync(gameId, gameActive.WhitePlayerId, gameActive.BlackPlayerId);

            await _hubContext.Clients.Group($"Game:{gameId}").SendAsync("GameEnded",
                new SignalRResponse<GameEndedResponse>(
                    Type: "GameEnded",
                    CorrelationId: gameId,
                    Data: new GameEndedResponse(gameId, winner is not null ? winner.Nickname : null, reason)
                ));

            return new EmptyResponse();
        }

        private async Task MakeAIMoveAsync(GameActive gameActive, string fen = DefaultFen)
        {

            if (gameActive == null)
                return;


            var bestMove = await _chessEngine.GetMoveAsync(fen, gameActive.AiDifficulty * 1000);
            if(bestMove.IsError)
            {
                await SurrenderGameAsync(_aiPlayerId, gameActive.Id);
                return;
            }

            var makeMoveRequest = new SignalRRequest<MakeMovePayload>(
                Type: "MakeMove",
                CorrelationId: gameActive.Id,
                Payload: new MakeMovePayload
                (
                    GameId: gameActive.Id,
                    San: bestMove.Value 
                ));

            await MakeMoveAsync(_aiPlayerId, makeMoveRequest); 
        }
    }
}
