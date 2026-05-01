using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class RemoveFriendsTests : UsersServiceTestBase
{
    [Fact]
    public async Task ShouldReturnOK_WhenFriendIsRemoved()
    {
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);
        await SeedAsync(MakeFriendship(user, friend), MakeFriendship(friend, user));

        var result = await _usersService.RemoveFriendAsync(friend.Nickname, user.Nickname);
        result.IsError.Should().BeFalse();

        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == friend.Id);
        friendship.Should().BeNull();

    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenFriendDoesNotExist()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
        var result = await _usersService.RemoveFriendAsync("nonexistent", user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var friend = MakeUser("bob");
        await SeedAsync(friend);
      
        var result = await _usersService.RemoveFriendAsync(friend.Nickname, "nonexistent");
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenUserIsNotAFriend()
    {
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);
      
      
        var result = await _usersService.RemoveFriendAsync(friend.Nickname, user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("friendshipNotFound");
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenUserTriesToRemoveHimself()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
  
        var result = await _usersService.RemoveFriendAsync(user.Nickname, user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("sameUser");
    }
}