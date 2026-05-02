namespace backend.DTO.Home;

public record HomeInfoResponse(
    int UsersOnline,
    int UsersInQueue,
    int ActiveGames,
    int TotalUsers
);