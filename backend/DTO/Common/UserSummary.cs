namespace backend.DTO.Common;

public record UserSummary(
    string Nickname,
    string ProfilePictureUrl,
    int Rating
);