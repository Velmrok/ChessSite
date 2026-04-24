

using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

public class GetFriendsTests : TestBase
{
    public GetFriendsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetFriends_ShouldReturnOk()
    {
        await LoginAsUserAsync();
        var user = await MakeUserAsync("testuser2@example.com", "testuser2", "TestUser2", "1234562");
        var response = await _client.GetAsync($"/users/{user.Nickname}/friends");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    

}