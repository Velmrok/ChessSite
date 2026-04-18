using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Users
{
    public record UserResponse(
        string Nickname,
        string ProfileBio,
        string ProfilePictureUrl,
        DateTime CreatedAt,
        DateTime LastActive,
        RoleType Role,
        RatingResponse Rating);
}