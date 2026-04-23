using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class Friendship
{
    public required Guid UserId { get; set; }
    public required Guid FriendId { get; set; }

   
    public User User { get; set; } = null!;
    
    public User Friend { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}