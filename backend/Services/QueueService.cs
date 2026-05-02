


using backend.Data;
using backend.DTO.Queue;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace backend.Services;

public class QueueService : IQueueService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly AppDbContext _dbContext;
    private const string QueueSetKey = "chess:queue";

    public QueueService(IConnectionMultiplexer redis, AppDbContext dbContext)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> JoinQueueAsync(string? userId, JoinQueuePayload payload)
    {
        if (userId == null)
            return Error.Failure("userNotAuthenticated");
        if ( Guid.TryParse(userId, out var userGuid) == false)
            return Error.Failure("invalidUserId");

        var exists = await _db.SetContainsAsync(QueueSetKey, $"{userId}");
        if (exists)
            return Error.Failure("userAlreadyInQueue");


        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userGuid);
        if (user == null)
            return Error.Failure("userNotFound");

        await _db.HashSetAsync($"{QueueSetKey}:{userId}",
        [
            new("time", payload.Time),
            new("increment", payload.Increment),
            new("rating", user.GetRatingByTime(payload.Time))
        ]);
        await _db.SetAddAsync(QueueSetKey, $"{userId}");

        return new Success();
    }

    public async Task<ErrorOr<Success>> LeaveQueueAsync(string? userId)
    {
        if (userId == null)
            return Error.Failure("userNotAuthenticated");

        await _db.KeyDeleteAsync($"{QueueSetKey}:{userId}");
        await _db.SetRemoveAsync(QueueSetKey, $"{userId}");
        return new Success();
    }

    public async Task<int> GetQueueLengthAsync()
    {
        return (int) await _db.SetLengthAsync(QueueSetKey);
    }
}