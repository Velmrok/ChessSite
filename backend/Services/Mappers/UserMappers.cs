using backend.DTO.Auth;
using backend.DTO.Common;
using backend.DTO.Users;
using backend.Models;

namespace backend.Services.Mappers;
public static class UserMappers
{
    public static GetMeResponse ToGetMeResponse(this User user)
    {
        return new GetMeResponse(
            Nickname: user.Nickname,
            Login: user.Login,
            Email: user.Email,
            ProfileBio: user.ProfileBio,
            ProfilePictureUrl: user.ProfilePictureUrl,
            CreatedAt: user.CreatedAt,
            LastActive: user.LastActive,
            Rating: new RatingResponse(user.RapidRating, user.BlitzRating, user.BulletRating)
        );
    }
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(
            Nickname: user.Nickname,
            ProfileBio: user.ProfileBio,
            ProfilePictureUrl: user.ProfilePictureUrl,
            CreatedAt: user.CreatedAt,
            LastActive: user.LastActive,
            Role: user.Role,
            Rating: new RatingResponse(user.RapidRating, user.BlitzRating, user.BulletRating)
        );
    }
 
}