

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
        var response = await _client.PatchAsJsonAsync("/users/me/profile/bio", new UpdateUserBioRequest(newBio));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var updatedProfile = await response.Content.ReadFromJsonAsync<UpdateUserBioResponse>();
        updatedProfile.Should().NotBeNull();
        updatedProfile.Bio.Should().Be(newBio);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenBioIsTooLong()
    {
        await LoginAsUserAsync();
        var longBio = new string('a', 2010);
        var response = await _client.PatchAsJsonAsync("/users/me/profile/bio", new UpdateUserBioRequest(longBio));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
 

}