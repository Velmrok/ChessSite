
using backend.DTO.Common;
using backend.DTO.Users;

namespace backend.DTO.Users;
public record FriendsProfileResponse(
    List<FriendProfileSummary> Friends,
    int TotalPages
    );


public record FriendProfileSummary
(
    string Nickname,
    string ProfilePictureUrl,
    RatingStats Rating,
    bool IsOnline
);
