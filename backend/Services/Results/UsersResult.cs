
using backend.DTO.Users;

namespace backend.Services.Results;

public record UsersResult(
    UsersResponse Response,
    bool IsCached
);