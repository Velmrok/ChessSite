

using System.Net.Http.Json;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;


public class RemoveFriendTests : TestBase
{
    public RemoveFriendTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    
   
    [Fact]
    public async Task ShouldReturn204NoContent_WhenFriendshipExists()
    {
        var userId = await LoginAsUserAsync();
        
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");

        _dbContext.Friendships.Add(new Friendship
        {
            UserId = userId,
            FriendId = user1.Id
        });
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.DeleteAsync($"/users/{user1.Nickname}/friend");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenFriendDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.DeleteAsync($"/users/nonexistentuser/friend");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task ShouldReturnValidationError_WhenRemovingSelf()
    {
        await LoginAsUserAsync();
        var response = await _client.DeleteAsync($"/users/TestNick/friend");
        var content = await response.Content.ReadAsStringAsync();
Console.WriteLine(content);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ShouldReturnConflict_WhenAreNotFriends()
    {
        await LoginAsUserAsync();
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");

        var response = await _client.DeleteAsync($"/users/{user1.Nickname}/friend");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }
}