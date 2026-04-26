using backend.DTO.Common;
using backend.Enums;

namespace backend.DTO.Users
{
    public record UsersSearchResponse(
        List<UserSearchSummary> Users,
        int TotalPages
        );
    public record UserSearchSummary(
   string Nickname,
   string ProfilePictureUrl,
   bool IsOnline,
   DateTime CreatedAt,
   DateTime LastActive,
   RoleType Role,
   RatingStats Rating);
}