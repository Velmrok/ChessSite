

using System.Net.Http.Json;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;


public class GetFriendsTests : TestBase
{
    public GetFriendsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldReturnOk()
    {
        await LoginAsUserAsync();
        var user = await MakeUserAsync("testuser2@example.com", "testuser2", "TestUser2", "1234562");
        var response = await _client.GetAsync($"/users/{user.Nickname}/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/users/nonexistent/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoFriends()
    {
        await LoginAsUserAsync();
        var user = await MakeUserAsync("testuser3@example.com", "testuser3", "TestUser3", "1234563");
        var result = await _client.GetAsync($"/users/{user.Nickname}/friends");
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var res = await result.Content.ReadFromJsonAsync<FriendsResponse>();
        res.Should().NotBeNull();
        res.Friends.Should().BeEmpty();
        res.TotalPages.Should().Be(0);
    }
    [Fact]
    public async Task ShouldReturnForbidden_WhenUserIsNotAuthenticated()
    {
        var response = await _client.GetAsync("/users/TestNick/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    

}