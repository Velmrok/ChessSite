namespace backend.DTO.Games;

public record UserGameSummary(
    string Nickname,
    string ProfilePictureUrl,
    int Rating
);