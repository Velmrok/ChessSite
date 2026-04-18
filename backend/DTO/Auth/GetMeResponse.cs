using backend.DTO.Common;

namespace backend.DTO.Auth;



public record GetMeResponse(
    string Nickname,
    string Login,
    string Email,
    string ProfileBio,
    string ProfilePictureUrl,
    DateTime CreatedAt,
    DateTime LastActive,
    RatingResponse Rating  
);