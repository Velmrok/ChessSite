using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class AddFriendsTests : UsersServiceTestBase
{
    [Fact]
    public async Task ShouldReturnOK_WhenFriendIsAdded()
    {
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);

        var result = await _usersService.AddFriendAsync(friend.Nickname, user.Nickname);
        result.IsError.Should().BeFalse();

        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == friend.Id);
        friendship.Should().NotBeNull();

        var reverseFriendship = await _dbContext.Friendships.FirstOrDefaultAsync(f => f.UserId == friend.Id && f.FriendId == user.Id);
        reverseFriendship.Should().NotBeNull();

    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenFriendDoesNotExist()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
        var result = await _usersService.AddFriendAsync("nonexistent", user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var friend = MakeUser("bob");
        await SeedAsync(friend);
      
        var result = await _usersService.AddFriendAsync(friend.Nickname, "nonexistent");
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenFriendIsAlreadyAdded()
    {
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);
        await SeedAsync(MakeFriendship(user, friend), MakeFriendship(friend, user));
      
        var result = await _usersService.AddFriendAsync(friend.Nickname, user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("alreadyFriends");
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenUserTriesToAddHimself()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
  
        var result = await _usersService.AddFriendAsync(user.Nickname, user.Nickname);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("sameUser");
    }
}