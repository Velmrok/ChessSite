using backend.DTO.Auth;
using backend.DTO.Common;
using backend.Enums;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nickname { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public  string ProfileBio { get; set; } = "";
    public  string ProfilePictureUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public int RapidRating { get; set; }
    public int BlitzRating { get; set; }
    public int BulletRating { get; set; }
    public RoleType Role { get; set; } = RoleType.User;


    public GetMeResponse ToGetMeResponse()
    {
        return new GetMeResponse(
            Nickname: Nickname,
            Login: Login,
            Email: Email,
            ProfileBio: ProfileBio,
            ProfilePictureUrl: ProfilePictureUrl,
            CreatedAt: CreatedAt,
            LastActive: LastActive,
            Rating: new RatingResponse(RapidRating, BlitzRating, BulletRating)
        );
    }
}