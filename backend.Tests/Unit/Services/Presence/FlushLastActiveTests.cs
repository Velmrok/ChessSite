using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using StackExchange.Redis;

namespace backend.Tests.Unit.Services.Presence;

public class FlushLastActiveTests : PresenceServiceTestBase
{
    
    [Fact]
    public async Task FlushLastActiveAsync_FlushesLastActiveTimestamps()
    {
        var user1 = MakeUser("user1", lastActive: DateTime.UtcNow.AddMinutes(-30));
        var user2 = MakeUser("user2", lastActive: DateTime.UtcNow.AddMinutes(-30));
        await SeedAsync(user1, user2);
        user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == "user1");
        user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == "user2");
        user1.Should().NotBeNull();
        user2.Should().NotBeNull();

        var server = Substitute.For<IServer>();
        _redis.GetEndPoints().Returns([new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 6379)]);
        _redis.GetServer(Arg.Any<System.Net.EndPoint>()).Returns(server);
        server.Keys(pattern:Arg.Any<RedisValue>()).Returns(
        [
            $"user:lastActive:{user1.Id}",
            $"user:lastActive:{user2.Id}"
        ]);

        var now = DateTime.UtcNow;
        var fiveMinutesAgo = now.AddMinutes(-5);


        _db.StringGetDeleteAsync($"user:lastActive:{user1.Id}").Returns(new RedisValue(now.ToString("O")));
        _db.StringGetDeleteAsync($"user:lastActive:{user2.Id}").Returns(new RedisValue(fiveMinutesAgo.ToString("O")));

       
        await _presenceService.FlushLastActiveAsync();
        await _db.Received(2).StringGetDeleteAsync(Arg.Any<RedisKey>());
      
        _dbContext.ChangeTracker.Clear();

        user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user1.Id);
        user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user2.Id);
        user1.Should().NotBeNull();
        user2.Should().NotBeNull();
        user1.LastActive.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        user2.LastActive.Should().BeCloseTo(fiveMinutesAgo, TimeSpan.FromSeconds(1));
    }
}