using backend.Enums;

namespace backend.Models;
public class User
{
    public int Id { get; set; }
    public required string Nickname { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string ProfileBio { get; set; }
    public required string ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
    public int RapidRating { get; set; }
    public int BlitzRating { get; set; }
    public int BulletRating { get; set; }
    public RoleType Role { get; set; }
}