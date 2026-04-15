

namespace backend.DTO.Auth;
public record AuthResponse(string AccessToken, string? RefreshToken = null);
