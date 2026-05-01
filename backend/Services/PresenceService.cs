using backend.Data;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using StackExchange.Redis;



public class PresenceService : IPresenceService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly AppDbContext _dbContext;
    private const string OnlineSetKey = "users:online";
    public PresenceService(IConnectionMultiplexer redis, AppDbContext dbContext)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _dbContext = dbContext;
    }

    public async Task SetOnlineAsync(Guid userId)
    {
        await _db.StringSetAsync($"user:online:{userId}", "1", TimeSpan.FromSeconds(30));
        await _db.SetAddAsync(OnlineSetKey, userId.ToString());
        await _db.StringSetAsync($"user:lastActive:{userId}", DateTime.UtcNow.ToString("O")); // ← to
    }


    public async Task<int> GetOnlineCountAsync()
    {
        return (int)await _db.SetLengthAsync(OnlineSetKey);
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        return await _db.SetContainsAsync(OnlineSetKey, userId.ToString());
    }

    public async Task<HashSet<Guid>> GetOnlineIdsAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.ToList();
        if (ids.Count == 0) return [];

        var tasks = ids.Select(id => _db.SetContainsAsync(OnlineSetKey, id.ToString()));
        var results = await Task.WhenAll(tasks);

        return [.. ids
            .Zip(results)
            .Where(x => x.Second)
            .Select(x => x.First)];
    }

    public async Task CleanUpAsync()
    {
        foreach (var member in await _db.SetMembersAsync(OnlineSetKey))
        {
            if (!await _db.KeyExistsAsync($"user:online:{member}"))
        {
            await _db.SetRemoveAsync(OnlineSetKey, member);
            }
        }
    }
    public async Task FlushLastActiveAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "user:lastActive:*").ToList();

        if (!keys.Any()) return;

        var updates = new List<(Guid userId, DateTime lastActive)>();
        
        foreach (var key in keys)
        {
            
            var value = await _db.StringGetDeleteAsync(key);
            if (!value.HasValue) continue;
            
            var userId = Guid.Parse(key.ToString().Split(':')[2]);
            var lastActive = DateTimeOffset.Parse(value.ToString()).UtcDateTime;
            
            updates.Add((userId, lastActive));
        }

        var ids = updates.Select(u => u.userId).ToList();
        var times = updates.Select(u => u.lastActive).ToArray();

        var lookup = ids.Zip(times).ToDictionary(x => x.First, x => x.Second);

        var updatesZipped = ids.Zip(times, (id, time) => new { id, time });
        
        foreach (var update in updatesZipped)
        {
            await _dbContext.Users
                .Where(u => u.Id == update.id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastActive, update.time));
            
        }
        ;
    }

}