
using System.Net.Http.Json;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.DTO.Users;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace backend.Tests.Integration.Home;
public class GetLeaderboardTests : TestBase
{
    public GetLeaderboardTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    [Fact]
    public async Task ShouldReturnOk()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/home/leaderboard");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task ShouldReturnLeaderboardData()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/home/leaderboard");
        var leaderboardData = await response.Content.ReadFromJsonAsync<LeaderBoardResponse>();
        leaderboardData.Should().NotBeNull();
        leaderboardData.TopRapidPlayers.Should().HaveCount(1);
        leaderboardData.TopBlitzPlayers.Should().HaveCount(1);
        leaderboardData.TopBulletPlayers.Should().HaveCount(1);
        leaderboardData.TopRapidPlayers[0].Nickname.Should().Be("TestNick");
        leaderboardData.TopBlitzPlayers[0].Nickname.Should().Be("TestNick");
        leaderboardData.TopBulletPlayers[0].Nickname.Should().Be("TestNick");

    }
    [Fact]
    public async Task ShouldReturnLeaderboardData_CorrectlySorted()
    {
        var user1 = await MakeUserAsync(nickname: "User1", rating: new RatingStats(1500, 1400, 1300)); 
        var user2 = await MakeUserAsync(nickname: "User2", rating: new RatingStats(1600, 1500, 1400));
        var user3 = await MakeUserAsync(nickname: "User3", rating: new RatingStats(1400, 1300, 1200));
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/home/leaderboard");
        var leaderboardData = await response.Content.ReadFromJsonAsync<LeaderBoardResponse>();
        leaderboardData.Should().NotBeNull();
        leaderboardData.TopRapidPlayers.Should().HaveCount(4);
        leaderboardData.TopBlitzPlayers.Should().HaveCount(4);
        leaderboardData.TopBulletPlayers.Should().HaveCount(4);

        leaderboardData.TopRapidPlayers[0].Nickname.Should().Be("User2");
        leaderboardData.TopRapidPlayers[1].Nickname.Should().Be("User1");
        leaderboardData.TopRapidPlayers[2].Nickname.Should().Be("User3");

        leaderboardData.TopBlitzPlayers[0].Nickname.Should().Be("User2");
        leaderboardData.TopBlitzPlayers[1].Nickname.Should().Be("User1");
        leaderboardData.TopBlitzPlayers[2].Nickname.Should().Be("User3");

        leaderboardData.TopBulletPlayers[0].Nickname.Should().Be("User2");
        leaderboardData.TopBulletPlayers[1].Nickname.Should().Be("User1");
        leaderboardData.TopBulletPlayers[2].Nickname.Should().Be("User3");
    }
}