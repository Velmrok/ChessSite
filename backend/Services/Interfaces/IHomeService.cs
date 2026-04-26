using ErrorOr;
using backend.DTO.Home;
namespace backend.Services.Interfaces;
public interface IHomeService
{
    Task<ErrorOr<LeaderBoardResponse>> GetLeaderboardAsync();
    Task<ErrorOr<Success>> JoinHomeGroupAsync(string id);
}