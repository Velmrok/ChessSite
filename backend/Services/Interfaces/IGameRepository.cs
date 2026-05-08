using backend.DTO.Games;
using backend.Models;
using ErrorOr;

namespace backend.Services.Interfaces;

public interface IGameRepository
{
    Task<GameActive?> GetActiveGameAsync(string gameId);
    Task<List<MoveInfo>> GetMovesByGameIdAsync(string gameId);
    Task<bool> IsActiveGameAsync(string gameId);
    Task<bool> IsInGameAsync(Guid userId);
    Task<string?> GetActiveGameIdForUserAsync(Guid userId);
    Task<bool> CreateActiveGameAsync(GameActive game, string whitePlayerId, string blackPlayerId, bool setUserActiveKeys);
    Task<ErrorOr<Success>> ExecuteMakeMoveScriptAsync(string gameId, string userId, string serializedMove, bool newIsWhiteTurn, int newWhiteTime, int newBlackTime);
    Task CleanupActiveGameAsync(string gameId, string whitePlayerId, string blackPlayerId);
    Task SaveFinishedGameAsync(Game game);
    Task<Game?> GetFinishedGameByIdAsync(string gameId);
    Task<(List<Game> Games, int TotalPages)> GetFinishedGamesPagedAsync(GetGamesQuery query);
    Task RemoveStaleActiveGameAsync(string gameId);

    Task SetUserAiGameAsync(string userId, string gameId);
    Task<string?> GetUserAiGameAsync(string userId);
    Task RemoveUserAiGameAsync(string userId);
    Task<int> GetActiveAiGamesCountAsync();
}
