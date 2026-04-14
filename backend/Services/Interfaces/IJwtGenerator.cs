using backend.Models;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}