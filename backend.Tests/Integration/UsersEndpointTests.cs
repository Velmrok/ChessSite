

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
        var request = new RegisterRequest
        {
            Email = "nowy@test.com",
            Login = "nowyUser",
            Nickname = "nowyKozak",
            Password = "123456"
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        response = await _client.GetAsync("/users");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var jsonData = await response.Content.ReadFromJsonAsync<UsersResponse>();

        jsonData.Should().NotBeNull();
        var users = jsonData.Users;

        users.Should().NotBeNull();
        users.Should().HaveCount(1);
        users[0].Nickname.Should().Be("nowyKozak");
    }
   
}