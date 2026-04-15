
using Microsoft.EntityFrameworkCore;

namespace backend.Models;
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required User User{ get; set; }
    
}