using FluentAssertions;
using NSubstitute;
using StackExchange.Redis;

namespace backend.Tests.Unit.Services.Presence;

public class CleanUpTests : PresenceServiceTestBase
{
    
    [Fact]
    public async Task CleanUpAsync_RemovesOfflineUsers()
    {
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
    
        _db.SetMembersAsync(OnlineSetKey).Returns(
        [
            userId.ToString(),
            anotherUserId.ToString()
        ]);
        _db.KeyExistsAsync($"user:online:{anotherUserId}").Returns(false);
        _db.KeyExistsAsync($"user:online:{userId}").Returns(true);
        await _presenceService.CleanUpAsync();
        
        await _db.Received(0).SetRemoveAsync(OnlineSetKey, userId.ToString());
        await _db.Received(1).SetRemoveAsync(OnlineSetKey, anotherUserId.ToString());
    }
}