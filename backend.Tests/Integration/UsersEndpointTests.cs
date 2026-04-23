

using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

public class UsersEndpointTests : TestBase
{
    public UsersEndpointTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    

    [Fact]
    public async Task GetUsersEndpoint_WithNoUsers_ShouldReturnOk()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/users");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var jsonData = await response.Content.ReadFromJsonAsync<UsersResponse>();
        jsonData.Should().NotBeNull();
        var users = jsonData.Users;
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUsersEndpoint_WithUsers_ShouldReturnOk()
    {
        await LoginAsUserAsync();

        var response = await _client.GetAsync("/users");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var jsonData = await response.Content.ReadFromJsonAsync<UsersResponse>();

        jsonData.Should().NotBeNull();
        var users = jsonData.Users;

        users.Should().NotBeNull();
        users.Should().HaveCount(1);
        users[0].Nickname.Should().Be("TestNick");
    }
    [Fact]
    public async Task GetUsersEndpoint_ShouldReturnNotCachedResponse()
    {
        await LoginAsUserAsync();

        var getResponse = await _client.GetAsync("/users");
        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        getResponse.Headers.Contains("X-Cache").Should().BeTrue();
        getResponse.Headers.GetValues("X-Cache").First().Should().Be("MISS");

        var secondResponse = await _client.GetAsync("/users");
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        secondResponse.Headers.Contains("X-Cache").Should().BeTrue();
        secondResponse.Headers.GetValues("X-Cache").First().Should().Be("HIT");
    }
    [Fact]
    public async Task GetUsersEndpoint_WithDifferentQueries_ShouldNotCache()
    {
        await LoginAsUserAsync();


        var response = await _client.GetAsync("/users?search=nonexistent");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Headers.Contains("X-Cache").Should().BeTrue();
        response.Headers.GetValues("X-Cache").First().Should().Be("MISS");


        response = await _client.GetAsync("/users?search=different");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Headers.Contains("X-Cache").Should().BeTrue();
        response.Headers.GetValues("X-Cache").First().Should().Be("MISS");
    }
    [Fact]
    public async Task GetUsersEndpoint_ShouldCacheResponses()
    {
        await LoginAsUserAsync();

        var response = await _client.GetAsync("/users?search=test");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Headers.Contains("X-Cache").Should().BeTrue();
        response.Headers.GetValues("X-Cache").First().Should().Be("MISS");

        var secondResponse = await _client.GetAsync("/users?search=test");
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        secondResponse.Headers.Contains("X-Cache").Should().BeTrue();
        secondResponse.Headers.GetValues("X-Cache").First().Should().Be("HIT");
    }
    [Fact]
    public async Task GetUsersEndpoint_ShouldReturnNotCachedResponse_AfterUserRegistration()
    {
        await LoginAsUserAsync();

        var response = await _client.GetAsync("/users?search=test");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Headers.Contains("X-Cache").Should().BeTrue();
        response.Headers.GetValues("X-Cache").First().Should().Be("MISS");

        var secondResponse = await _client.GetAsync("/users?search=test");
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        secondResponse.Headers.Contains("X-Cache").Should().BeTrue();
        secondResponse.Headers.GetValues("X-Cache").First().Should().Be("HIT");

        await RegisterUserAsync(email: "newuser@example.com", login: "newuser", nickname: "NewUser", password: "123456");

        var thirdResponse = await _client.GetAsync("/users?search=test");
        thirdResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        thirdResponse.Headers.Contains("X-Cache").Should().BeTrue();
        thirdResponse.Headers.GetValues("X-Cache").First().Should().Be("MISS");
    }
    [Fact]
    public async Task GetUsersEndpoint_ShouldReturnOnlyOnlineUsers_WhenFilterOnlineIsTrue()
    {
        await LoginAsUserAsync();
        var onlineUser = await MakeUserAsync("onlineuser@example.com", "onlineuser", "OnlineUser", "123456");

        await _cache.SetStringAsync($"user:online:{onlineUser.Id}", "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
         });

        var response = await _client.GetAsync("/users?JustOnline=true");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var jsonData = await response.Content.ReadFromJsonAsync<UsersResponse>();

        jsonData.Should().NotBeNull();
        var users = jsonData.Users;

        users.Should().NotBeNull();
        users.Should().HaveCount(1);
        users.First().Nickname.Should().Be(onlineUser.Nickname);
    }
    [Fact]
    public async Task GetUserProfile_ShouldReturnUserProfile_WhenUserExists()
    {
        await LoginAsUserAsync();
       
        var response = await _client.GetAsync($"/users/{"TestNick"}/profile");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        profile.Should().NotBeNull();
        profile.Nickname.Should().Be("TestNick");
       
    }
    [Fact]
    public async Task GetUserProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/users/nonexistentuser");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task GetFriends_ShouldReturnOk()
    {
        await LoginAsUserAsync();
        var user = await MakeUserAsync("testuser2@example.com", "testuser2", "TestUser2", "1234562");
        var response = await _client.GetAsync($"/users/{user.Nickname}/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task GetFriends_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.GetAsync("/users/nonexistentuser/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task AddFriend_ShouldReturn204NoContent_WhenUsersExist()
    {
        await LoginAsUserAsync();
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");
        
        
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { user1.Nickname }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task AddFriend_ShouldReturnNotFound_WhenFriendDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = "nonexistentuser" }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task AddFriend_ShouldReturnValidationError_WhenAddingSelf()
    {
        await LoginAsUserAsync();
        var response = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = "TestNick" }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task AddFriend_ShouldReturnConflict_WhenAlreadyFriends()
    {
        await LoginAsUserAsync();
        var user1 = await MakeUserAsync("user1@example.com", "user1", "User1", "123456");

        
        var response1 = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = user1.Nickname }));
        response1.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        var response2 = await _client.PostAsync($"/users/friend", JsonContent.Create(new { Nickname = user1.Nickname }));
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task UpdateUserBio_ShouldReturnOk_WhenBioIsUpdated()
    {
        await LoginAsUserAsync();
        var newBio = "This is my new bio.";
        var response = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest(newBio));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var updatedProfile = await response.Content.ReadFromJsonAsync<UpdateUserBioResponse>();
        updatedProfile.Should().NotBeNull();
        updatedProfile.Bio.Should().Be(newBio);
    }
    [Fact]
    public async Task UpdateUserBio_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest("New bio"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var response2 = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest("New bio"));
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task UpdateUserBio_ShouldReturnBadRequest_WhenBioIsTooLong()
    {
        await LoginAsUserAsync();
        var longBio = new string('a', 2010);
        var response = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest(longBio));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task UpdateUserBio_ShouldReturnForbidden_WhenUpdatingOtherUserBio()
    {
        await LoginAsUserAsync();
        var otherUser = await MakeUserAsync("otheruser@example.com", "otheruser", "OtherUser", "123456");
        var response = await _client.PatchAsJsonAsync($"/users/{otherUser.Nickname}/profile/bio", new UpdateUserBioRequest("New bio"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

}