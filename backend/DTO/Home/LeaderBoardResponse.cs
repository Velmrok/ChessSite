
using backend.DTO.Common;

namespace backend.DTO.Home;

public record LeaderBoardResponse(
    List<UserLeaderboardSummary> TopRapidPlayers,
    List<UserLeaderboardSummary> TopBlitzPlayers,
    List<UserLeaderboardSummary> TopBulletPlayers
);
