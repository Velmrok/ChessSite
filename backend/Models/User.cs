using backend.Enums;

namespace backend.Models;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nickname { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string ProfileBio { get; set; }
    public required string ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public int RapidRating { get; set; }
    public int BlitzRating { get; set; }
    public int BulletRating { get; set; }
    public RoleType Role { get; set; }
}