

using System.Net.Http.Json;
using backend.DTO.Users;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;


public class GetOnlineFriendsTests : TestBase
{
    public GetOnlineFriendsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    [Fact]
    public async Task ShouldReturnFriendsList_WhenUserHasOnlineFriends()
    {
        await LoginAsUserAsync();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        var user2 = await MakeUserAsync("testuser3@example.com", "testuser3", "TestUser3", "1234563");
        var friendship = await MakeFriendshipAsync(user!, user2);
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns([user2.Id]);
        var response = await _client.GetAsync($"/users/me/friends-online");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoOnlineFriends()
    {
        await LoginAsUserAsync();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        var user2 = await MakeUserAsync("testuser4@example.com", "testuser4", "TestUser4", "1234564");
        var friendship = await MakeFriendshipAsync(user!, user2);
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns([]);
        var response = await _client.GetAsync($"/users/me/friends-online");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenUserIsNotAuthenticated()
    {
        var response = await _client.GetAsync("/users/me/friends-online");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}