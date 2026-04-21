// backend/Services/Helpers/CacheInvalidationService.cs

using backend.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services.Helpers.Auth;

public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly IDistributedCache _cache;

    public CacheInvalidationService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task InvalidateUsersCache()
    {
        var versionString = await _cache.GetStringAsync("users:version");
        if (!int.TryParse(versionString, out int version)) version = 0;
        version++;
        await _cache.SetStringAsync("users:version", version.ToString());
    }
}