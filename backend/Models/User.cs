using backend.DTO.Common;
using backend.Enums;


namespace backend.Models;
public class User
{
    public Guid Id { get; set; }
    public required string Nickname { get; set; }
    public string ProfileBio { get; set; } = "";
    public string ProfilePictureUrl { get; set; } = "/avatar-default.webp";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public RatingStats Rating { get; set; } = new RatingStats(1200, 1200, 1200);
    public RatingStats Wins { get; set; } = new RatingStats(0, 0, 0);
    public RatingStats Losses { get; set; } = new RatingStats(0, 0, 0);
    public RatingStats Draws { get; set; } = new RatingStats(0, 0, 0);
    public int GamesPlayed { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public RoleType Role { get; set; } = RoleType.User;






}