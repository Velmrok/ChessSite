using backend.DTO.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class UploadProfilePictureTests : UsersServiceTestBase
{
    [Fact]
    public async Task ShouldUpdateProfilePicture_WhenRequestIsValid()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);

        _storageService
            .UploadAvatarAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("example/avatar.jpg");

        var fileMock = Substitute.For<IFormFile>();
        fileMock.ContentType.Returns("image/jpeg");
        fileMock.OpenReadStream().Returns(new MemoryStream([1, 2, 3]));

        var request = new UpdateUserProfilePictureRequest(ProfilePictureFile: fileMock);
        var result = await _usersService.UpdateUserProfilePictureAsync(user.Nickname, request);
        result.IsError.Should().BeFalse();
        var updatedUser = await _dbContext.Users.FindAsync(user.Id);
        updatedUser!.ProfilePictureUrl.Should().NotBeEmpty();
        updatedUser.ProfilePictureUrl.Should().Contain("example/avatar.jpg");
    }
    [Fact]
    public async Task ShouldReturnError_WhenUserDoesNotExist()
    {
        var fileMock = Substitute.For<IFormFile>();
        fileMock.ContentType.Returns("image/jpeg");
        fileMock.OpenReadStream().Returns(new MemoryStream([1, 2, 3]));

        var request = new UpdateUserProfilePictureRequest(ProfilePictureFile: fileMock);
        var result = await _usersService.UpdateUserProfilePictureAsync("nonexistent", request);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
}