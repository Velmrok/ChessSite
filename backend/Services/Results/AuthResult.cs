using backend.DTO.Auth;

namespace backend.Services.Results;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    GetMeResponse User
);