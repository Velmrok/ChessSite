

using System.Net.Http.Json;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;


public class AddFriendTests : TestBase
{
    public AddFriendTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    
   
    [Fact]
    public async Task ShouldReturn204NoContent_WhenUsersExist()
    {
        await LoginAsUserAsync();
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");
        
        
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { user1.Nickname }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenFriendDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = "nonexistentuser" }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task ShouldReturnValidationError_WhenAddingSelf()
    {
        await LoginAsUserAsync();
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = "TestNick" }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ShouldReturnConflict_WhenAlreadyFriends()
    {
        await LoginAsUserAsync();
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");

        
        var response1 = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = user1.Nickname }));
        response1.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        var response2 = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = user1.Nickname }));
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }
}