using NSubstitute;
using StackExchange.Redis;

namespace backend.Tests.Unit.Services.Presence;

public class SetOnlineTests : PresenceServiceTestBase
{
    [Fact]
    public async Task SetOnlineAsync_SetsUserOnline()
    {
        var userId = Guid.NewGuid();
        
        await _presenceService.SetOnlineAsync(userId);
        
        await _db.Received(1).StringSetAsync($"user:online:{userId}", "1", TimeSpan.FromSeconds(30));
        await _db.Received(1).SetAddAsync(OnlineSetKey, userId.ToString(), Arg.Any<CommandFlags>());
        await _db.Received(1).StringSetAsync($"user:lastActive:{userId}", Arg.Any<RedisValue>());
    
    }
}