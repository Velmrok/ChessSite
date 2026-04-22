using backend.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;



public class PresenceService : IPresenceService
{
    private readonly IDistributedCache _cache; 
    private static readonly TimeSpan OnlineTtl = TimeSpan.FromSeconds(30);

    public PresenceService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private static string Key(Guid userId) => $"user:online:{userId}";

    public async Task SetOnlineAsync(Guid userId)
    {
        await _cache.SetStringAsync(Key(userId), "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = OnlineTtl
        });
    }

    public async Task SetOfflineAsync(Guid userId)
    {
        await _cache.RemoveAsync(Key(userId));
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        return await _cache.GetStringAsync(Key(userId)) != null;
    }

    public async Task<HashSet<Guid>> GetOnlineIdsAsync(IEnumerable<Guid> userIds)
    {
        var result = new HashSet<Guid>();

        var checks = userIds.Select(async id =>
        {
            if (await _cache.GetStringAsync(Key(id)) != null)
                result.Add(id);
        });

        await Task.WhenAll(checks);
        return result;
    }
}