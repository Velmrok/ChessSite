using backend.DTO.Games;
using ErrorOr;

namespace backend.Services.Interfaces;
public interface IGamesService
{
    Task<ErrorOr<GamesResponse>> GetAllGamesAsync(GetGamesQuery query);
}