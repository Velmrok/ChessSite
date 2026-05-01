using System.Net.Http.Json;
using backend.DTO.Auth;
using backend.DTO.Users;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

public class UploadUserProfilePictureTests : TestBase
{
    public UploadUserProfilePictureTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldUploadProfilePicture_WhenRequestIsValid()
    {
        await LoginAsUserAsync();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        var imageContent = new ByteArrayContent([1, 2, 3]);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        var ProfilePictureFile = new MultipartFormDataContent
        {
            { imageContent, "ProfilePictureFile", "profile.jpg" }
        };
        var response = await _client.PatchAsync("/users/me/profile/picture", ProfilePictureFile);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdateUserProfilePictureResponse>();
        result.Should().NotBeNull();
        result.ProfilePictureUrl.Should().NotBeNullOrEmpty();
        result.ProfilePictureUrl.Should().Contain(user!.Id.ToString());
}
    [Fact]
    public async Task ShouldReturnBadRequest_WhenNoFileIsProvided()
    {
        await LoginAsUserAsync();
        var response = await _client.PatchAsync("/users/me/profile/picture", new MultipartFormDataContent());
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}   