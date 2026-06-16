using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services;

public class RedisTicketStore : ITicketStore
{
    private readonly IDistributedCache _cache;
    public RedisTicketStore(IDistributedCache cache) => _cache = cache;

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = "auth:" + Guid.NewGuid().ToString("N");
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        byte[] bytes = TicketSerializer.Default.Serialize(ticket);
        var opts = new DistributedCacheEntryOptions();
        if (ticket.Properties.ExpiresUtc is { } exp) opts.SetAbsoluteExpiration(exp);
        await _cache.SetAsync(key, bytes, opts); 
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        byte[]? bytes = await _cache.GetAsync(key);   // get z Redisa
        return bytes is null ? null : TicketSerializer.Default.Deserialize(bytes);
    }

    public Task RemoveAsync(string key) => _cache.RemoveAsync(key);
}