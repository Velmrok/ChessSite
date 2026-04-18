using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Users
{
    public record UserResponse(
        string Nickname,
        string Login,
        string Email,
        string ProfileBio,
        string ProfilePictureUrl,
        DateTime CreatedAt,
        DateTime LastActive,
        RoleType Role,
        RatingResponse Rating);
}