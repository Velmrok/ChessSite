

using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

public class GetUserProfileTests : TestBase
{
    public GetUserProfileTests(WebApplicationFactory<Program> factory) : base(factory)
    {
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
    

}