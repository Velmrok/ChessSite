
using backend.DTO.Common;
using backend.DTO.Users;

namespace backend.DTO.Users;

public record OnlineFriendsResponse(
    List<FriendOnlineSummary> Friends,
     int TotalPages
    );

public record FriendOnlineSummary
(
    string Nickname,
    string ProfilePictureUrl
);