

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
    public UsersEndpointTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) : base(factory, output)
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
}