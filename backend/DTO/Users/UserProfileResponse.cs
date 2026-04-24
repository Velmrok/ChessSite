
using backend.DTO.Common;

namespace backend.DTO.Users;

public record UserProfileResponse(
    string Nickname,
    string ProfilePictureUrl,
    string Bio,
    bool IsOnline,
    UserInfo UserInfo
);

public record UserInfo(
   RatingStats Rating,
   string CreatedAt,
   int GamesPlayed,
   RatingStats Wins,
   RatingStats Losses,
   RatingStats Draws,
    int TotalWins,
    int TotalLosses,
    int TotalDraws
);
