

using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

public class UsersEndpointTests : TestBase
{
    public UsersEndpointTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    private async Task LoginAsUserAsync(
        string email = "test@test.com",
        string login = "testuser",
        string nickname = "TestNick",
        string password = "123456")
    {

        var registerRequest = new RegisterRequest
        {
            Email = email,
            Login = login,
            Nickname = nickname,
            Password = password
        };
        var response = await _client.PostAsJsonAsync("/auth/register", registerRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        
    }

    [Fact]
    public async Task GetUsersEndpoint_WithNoUsers_ShouldReturnOk()
    {
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
    
   
}