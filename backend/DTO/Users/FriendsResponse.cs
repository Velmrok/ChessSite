
using backend.DTO.Common;
using backend.DTO.Users;

public record FriendsResponse(
    List<UserSummary> Friends,
    int TotalPages
    );