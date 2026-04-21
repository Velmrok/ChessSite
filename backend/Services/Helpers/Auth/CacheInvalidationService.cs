// backend/Services/Helpers/CacheInvalidationService.cs

using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

public class CacheInvalidationService : ICacheInvalidationService
{
   
    private readonly IConnectionMultiplexer _redis;

    public CacheInvalidationService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task InvalidateUsersCache()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "ChessSite:users:*");
        var db = _redis.GetDatabase();
        foreach (var key in keys)
        {
            await db.KeyDeleteAsync(key);
        }
    }
}