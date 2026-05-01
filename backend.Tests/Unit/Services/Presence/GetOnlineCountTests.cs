using FluentAssertions;
using NSubstitute;
using StackExchange.Redis;

namespace backend.Tests.Unit.Services.Presence;

public class GetOnlineCountTests : PresenceServiceTestBase
{
    [Fact]
    public async Task GetOnlineCountAsync_ReturnsCorrectCount()
    {
        _db.SetLengthAsync(Arg.Any<RedisKey>()).Returns(2);
        var onlineCount = await _presenceService.GetOnlineCountAsync();
        
        await _db.Received(1).SetLengthAsync(OnlineSetKey, Arg.Any<CommandFlags>());
        onlineCount.Should().Be(2);

    }
    [Fact]
    public async Task GetOnlineCountAsync_ReturnsZeroWhenNoUsersOnline()
    {
        _db.SetLengthAsync(Arg.Any<RedisKey>()).Returns(0);
        var onlineCount = await _presenceService.GetOnlineCountAsync();
        
        await _db.Received(1).SetLengthAsync(OnlineSetKey, Arg.Any<CommandFlags>());
        onlineCount.Should().Be(0);
    }
}