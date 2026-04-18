

using backend.DTO.Users;
using backend.Models;

namespace backend.DTO.Auth;
public record AuthResponse(string AccessToken, string? RefreshToken = null, UserResponse? User = null);
