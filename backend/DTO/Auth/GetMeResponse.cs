using backend.DTO.Common;

namespace backend.DTO.Auth;



public record GetMeResponse(
    string Nickname,
    string ProfileBio,
    string ProfilePictureUrl,
    DateTime CreatedAt,
    DateTime LastActive,
    RatingStats Rating ,
    List<string> FriendNicknames 
);