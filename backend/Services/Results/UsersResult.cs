
using backend.DTO.Users;

namespace backend.Services.Results;

public record UsersResult(
    UsersSearchResponse Response,
    bool IsCached
);