using backend.Models;
namespace backend.Services.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}