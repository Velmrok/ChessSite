
using backend.DTO.Common;

namespace backend.DTO.Home;

public record LeaderBoardResponse(
    List<UserSummary> TopRapidPlayers,
    List<UserSummary> TopBlitzPlayers,
    List<UserSummary> TopBulletPlayers
);
