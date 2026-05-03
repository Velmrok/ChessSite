


using backend.Data;
using backend.DTO.Auth;
using backend.DTO.Queue;
using backend.Enums;
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
        if (Guid.TryParse(userId, out var userGuid) == false)
            return Error.Failure("invalidUserId");

        var exists = await _db.SetContainsAsync(QueueSetKey, $"{userId}");
        if (exists)
            return Error.Failure("userAlreadyInQueue");


        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userGuid);
        if (user == null)
            return Error.Failure("userNotFound");

        await _db.HashSetAsync($"{QueueSetKey}:{userId}",
        [
            new ("joinedAt", DateTime.UtcNow.ToString("o")),
            new("time", payload.Time),
            new("increment", payload.Increment),
            new("rating", user.GetRatingByTime(payload.Time))
        ]);
        await _db.SetAddAsync(QueueSetKey, $"{userId}");
        
        await _db.SetAddAsync($"{QueueSetKey}:{payload.Time}:{payload.Increment}", $"{userId}");

        await _db.StringSetAsync($"{userId}:Key", $"{QueueSetKey}:{payload.Time}:{payload.Increment}");

        return new Success();
    }

    public async Task<ErrorOr<Success>> LeaveQueueAsync(string? userId)
    {
        if (userId == null)
            return Error.Failure("userNotAuthenticated");

        await _db.KeyDeleteAsync($"{QueueSetKey}:{userId}");
        await _db.SetRemoveAsync(QueueSetKey, $"{userId}");
        var queueKey = await _db.StringGetAsync($"{userId}:Key");

        if(!queueKey.IsNull)
            await _db.SetRemoveAsync(queueKey.ToString(), $"{userId}");
        await _db.KeyDeleteAsync($"{userId}:Key");
        
        return new Success();
    }

    public async Task<int> GetQueueLengthAsync()
    {
        return (int)await _db.SetLengthAsync(QueueSetKey);
    }
    public QueueData GetUserQueueData(string userId)
    {
        var data = _db.HashGetAll($"{QueueSetKey}:{userId}");
        if (data.Length == 0)
            return new QueueData(false);

        var time = data.FirstOrDefault(e => e.Name == "time").Value;
        var increment = data.FirstOrDefault(e => e.Name == "increment").Value;
        var rating = data.FirstOrDefault(e => e.Name == "rating").Value;
        var joinedAt = data.FirstOrDefault(e => e.Name == "joinedAt").Value;

        return new QueueData(
            IsInQueue: true,
            Time: (int?)time,
            Increment: (int?)increment,
            JoinedQueueAt: DateTime.Parse(joinedAt.ToString())
        );
    }

    
}