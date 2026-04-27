using backend.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;



public class PresenceService : IPresenceService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private const string OnlineSetKey = "users:online";
    public PresenceService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public async Task SetOnlineAsync(Guid userId)
    {
        await _db.StringSetAsync($"user:online:{userId}", "1", TimeSpan.FromSeconds(30));
        await _db.SetAddAsync(OnlineSetKey, userId.ToString());
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
                await _db.SetRemoveAsync(OnlineSetKey, member);
        }
    }


}