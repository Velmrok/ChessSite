using backend.DTO.Users;
using FluentAssertions;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class GetUserProfileTests : UsersServiceTestBase
{
    
    [Fact]
    public async Task ShouldReturnUserProfile_WhenUserExists()
    {
        var user = MakeUser("alice", bio: "Hello, I'm Alice!");
        await SeedAsync(user);
        var result = await _usersService.GetUserProfileAsync(user.Nickname);
        result.IsError.Should().BeFalse();
        result.Value!.Nickname.Should().Be(user.Nickname);
        result.Value.Bio.Should().Be(user.ProfileBio);
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var result = await _usersService.GetUserProfileAsync("nonexistentuser");
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "userNotFound");
    }
    [Fact]
    public async Task ShouldReturnUserProfileWithEmptyBio_WhenUserHasNoBio()
    {
        var user = MakeUser("bob", bio: "");
        await SeedAsync(user);
        var result = await _usersService.GetUserProfileAsync(user.Nickname);
        result.IsError.Should().BeFalse();
        result.Value.Bio.Should().BeNullOrEmpty();
    }
    [Fact]
    public async Task ShouldReturnUserProfileWithIsOnlineStatus_WhenUserIsOnline()
    {
        var user = MakeUser("charlie");
        await SeedAsync(user);
        _presenceService.IsOnlineAsync(Arg.Any<Guid>()).Returns(true);
        var result = await _usersService.GetUserProfileAsync(user.Nickname);
        result.IsError.Should().BeFalse();
    }
    [Fact]
    public async Task ShouldReturnUserProfileWithIsOfflineStatus_WhenUserIsOffline()
    {
        var user = MakeUser("charlie");
        await SeedAsync(user);
        _presenceService.IsOnlineAsync(Arg.Any<Guid>()).Returns(false);
        var result = await _usersService.GetUserProfileAsync(user.Nickname);
        result.IsError.Should().BeFalse();
    }

}