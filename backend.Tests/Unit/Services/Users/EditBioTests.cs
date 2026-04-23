using backend.DTO.Users;
using FluentAssertions;

namespace backend.Tests.Unit.Services.Users;

public class EditBioTests : UsersServiceTestBase
{
    [Fact]
    public async Task ShouldUpdateBio_WhenRequestIsValid()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
        var request = new UpdateUserBioRequest ( Bio : "This is my new bio." );
        var result = await _usersService.UpdateUserBioAsync(user.Nickname, request);
        result.IsError.Should().BeFalse();
        var updatedUser = await _dbContext.Users.FindAsync(user.Id);
        updatedUser!.ProfileBio.Should().Be(request.Bio);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var request = new UpdateUserBioRequest ( Bio : "This is my new bio." );
        var result = await _usersService.UpdateUserBioAsync("nonexistentuser", request);    
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "userNotFound");
    }
    
    
}