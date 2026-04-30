using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using FluentAssertions;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class GetOnlineFriendsTests : UsersServiceTestBase
{
    [Fact]
    public async Task ShouldReturnOK_WhenUserHasOnlineFriends()
    {
        var user = MakeUser("alice", lastActive: DateTime.UtcNow);
        var friend1 = MakeUser("bob", lastActive: DateTime.UtcNow);
        var friend2 = MakeUser("charlie", lastActive: DateTime.UtcNow.AddDays(-1));
        await SeedAsync(user, friend1, friend2);
        await SeedAsync(MakeFriendship(user, friend1), MakeFriendship(friend1, user));
        await SeedAsync(MakeFriendship(user, friend2), MakeFriendship(friend2, user));

        _presenceService.IsOnlineAsync(friend1.Id).Returns(true);
        _presenceService.IsOnlineAsync(friend2.Id).Returns(false);
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns([friend1.Id]);

        var query = new PaginationQuery(1, 10);
        var result = await _usersService.GetOnlineFriendsAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        var response = result.Value;
        response.Friends.Should().HaveCount(1);
        response.Friends[0].Nickname.Should().Be("bob");
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoOnlineFriends()
    {
        var user = MakeUser("alice", lastActive: DateTime.UtcNow);
        var friend1 = MakeUser("bob", lastActive: DateTime.UtcNow.AddDays(-1));
        var friend2 = MakeUser("charlie", lastActive: DateTime.UtcNow.AddDays(-1));
        await SeedAsync(user, friend1, friend2);
        await SeedAsync(MakeFriendship(user, friend1), MakeFriendship(friend1, user));
        await SeedAsync(MakeFriendship(user, friend2), MakeFriendship(friend2, user));

        _presenceService.IsOnlineAsync(friend1.Id).Returns(false);
        _presenceService.IsOnlineAsync(friend2.Id).Returns(false);
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns([]);

        var query = new PaginationQuery(1, 10);
        var result = await _usersService.GetOnlineFriendsAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        var response = result.Value;
        response.Friends.Should().HaveCount(0);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var query = new PaginationQuery(1, 10);
        var result = await _usersService.GetOnlineFriendsAsync("nonexistent", query);
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("userNotFound");
    }
    [Fact]
    public async Task ShouldReturnCorrectlyPaginatedResults()
    {
        var user = MakeUser("alice", lastActive: DateTime.UtcNow);
        var friends = Enumerable.Range(1, 9).Select(i => MakeUser($"friend{i}", lastActive: DateTime.UtcNow)).ToArray();
        await SeedAsync(user);
        await SeedAsync(friends);

        var friendships = new List<Friendship>();
        foreach (var friend in friends)
        {
            friendships.Add(MakeFriendship(user, friend));
            friendships.Add(MakeFriendship(friend, user));
            _presenceService.IsOnlineAsync(friend.Id).Returns(true);
           
        }
        await SeedAsync([..friendships]);
         
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns([..friends.Select(f => f.Id)]);

        var query = new PaginationQuery(4, 2);
        var result = await _usersService.GetOnlineFriendsAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        var response = result.Value;
        response.Friends.Should().HaveCount(2);
        response.Friends[0].Nickname.Should().Be("friend7");
    }
}