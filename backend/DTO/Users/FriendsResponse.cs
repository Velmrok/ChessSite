
using backend.DTO.Users;

public record FriendsResponse(
    List<UserResponse> Friends,
    int TotalPages
    );