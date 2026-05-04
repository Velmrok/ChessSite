using backend.DTO.Common;
using backend.DTO.Games;
using ErrorOr;

namespace backend.Services.Interfaces;
public interface IGamesService
{
    Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query);
    Task<ErrorOr<GameResponse>> GetGameByIdAsync(string gameId);
    Task<ErrorOr<string>> CreateGameAsync(string user1Id, string user2Id, int time, int increment);
    Task<bool> IsInGameAsync(Guid userId);
    Task<ErrorOr<EmptyResponse>> MakeMoveAsync(string? userId, SignalRRequest<MakeMoveRequest> request);
    Task<ErrorOr<EmptyResponse>> SurrenderGameAsync(string? userId, string gameId);
}