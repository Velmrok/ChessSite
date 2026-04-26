using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Users
{
    public record UserResponse(
        string Nickname,
        string ProfilePictureUrl,
        DateTime CreatedAt,
        DateTime LastActive,
        RoleType Role,
        RatingStats Rating);

    public record UsersResponse(
        List<UserResponse> Users,
        int TotalPages
        );
}