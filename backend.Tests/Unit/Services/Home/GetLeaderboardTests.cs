using backend.Enums;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace backend.Tests.Unit.Services.Home;

public class GetLeaderboardTests : HomeServiceTestBase
{
    
    [Fact]
    public async Task GetLeaderboardAsync_ReturnsCorrectData()
    {
        var user1 = MakeUser("User1", rapid: 1500, blitz: 1400, bullet: 1300);
        var user2 = MakeUser("User2", rapid: 1600, blitz: 1500, bullet: 1400);
        var user3 = MakeUser("User3", rapid: 1400, blitz: 1300, bullet: 1200);
        await SeedAsync(user1, user2, user3);

        var result = await _homeService.GetLeaderboardAsync();


        result.IsError.Should().BeFalse();
        var leaderboard = result.Value;
        leaderboard.TopRapidPlayers.Should().HaveCount(3);
        leaderboard.TopBlitzPlayers.Should().HaveCount(3);
        leaderboard.TopBulletPlayers.Should().HaveCount(3);
        
        leaderboard.TopRapidPlayers[0].Nickname.Should().Be("User2");
        leaderboard.TopRapidPlayers[1].Nickname.Should().Be("User1");
        leaderboard.TopRapidPlayers[2].Nickname.Should().Be("User3");
        
        leaderboard.TopBlitzPlayers[0].Nickname.Should().Be("User2");
        leaderboard.TopBlitzPlayers[1].Nickname.Should().Be("User1");
        leaderboard.TopBlitzPlayers[2].Nickname.Should().Be("User3");
        
        leaderboard.TopBulletPlayers[0].Nickname.Should().Be("User2");
        leaderboard.TopBulletPlayers[1].Nickname.Should().Be("User1");
        leaderboard.TopBulletPlayers[2].Nickname.Should().Be("User3");
    }

}