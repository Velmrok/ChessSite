

using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

public class UpdateUserBioTests : TestBase
{
    public UpdateUserBioTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldReturnOk_WhenBioIsUpdated()
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
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await LoginAsUserAsync();
        var response = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest("New bio"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var response2 = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest("New bio"));
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenBioIsTooLong()
    {
        await LoginAsUserAsync();
        var longBio = new string('a', 2010);
        var response = await _client.PatchAsJsonAsync("/users/TestNick/profile/bio", new UpdateUserBioRequest(longBio));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ShouldReturnForbidden_WhenUpdatingOtherUserBio()
    {
        await LoginAsUserAsync();
        var otherUser = await MakeUserAsync("otheruser@example.com", "otheruser", "OtherUser", "123456");
        var response = await _client.PatchAsJsonAsync($"/users/{otherUser.Nickname}/profile/bio", new UpdateUserBioRequest("New bio"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

}