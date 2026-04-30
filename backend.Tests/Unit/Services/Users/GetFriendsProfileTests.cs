using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using FluentAssertions;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class GetFriendsProfileTests : UsersServiceTestBase
{
    
    [Fact]
    public async Task ShouldReturnOK_WhenUserHasFriends_BothWays()
    {
       
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);
        await SeedAsync(MakeFriendship(user, friend), MakeFriendship(friend, user));
        

        var query = new PaginationQuery(1, 10);

        var result = await _usersService.GetFriendsProfileAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        FriendsProfileResponse response = result.Value;
        response.Friends.Should().HaveCount(1);
        response.Friends[0].Nickname.Should().Be("bob");

        var result2 = await _usersService.GetFriendsProfileAsync(friend.Nickname, query);
        result2.IsError.Should().BeFalse();
        FriendsProfileResponse response2 = result2.Value;
        response2.Friends.Should().HaveCount(1);
        response2.Friends[0].Nickname.Should().Be("alice");
    }
    [Fact]
    public async Task ShouldReturnOK_WhenUserHasHasNoFriends()
    {
       
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
         await SeedAsync(user, friend);


        var query = new PaginationQuery(1, 10);

        var result = await _usersService.GetFriendsProfileAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        FriendsProfileResponse response = result.Value;
        response.Friends.Should().HaveCount(0);

        var result2 = await _usersService.GetFriendsProfileAsync(friend.Nickname, query);
        result2.IsError.Should().BeFalse();
        FriendsProfileResponse response2 = result2.Value;
        response2.Friends.Should().HaveCount(0);
    }
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var query = new PaginationQuery(1, 10);

        var result = await _usersService.GetFriendsProfileAsync("nonexistent", query);
        result.IsError.Should().BeTrue();

        
        result.FirstError.Code.Should().Be("userNotFound");
    }
   [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoFriends()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
        var query = new PaginationQuery(1, 10);
        var result = await _usersService.GetFriendsProfileAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        FriendsProfileResponse response = result.Value;
        response.Friends.Should().HaveCount(0);
    }
    [Fact]
    public async Task ShouldReturnPaginatedFriends()
    {
        var user = MakeUser("alice");
        await SeedAsync(user);
        for (int i = 0; i < 15; i++)
        {
            var friend = MakeUser($"friend{i}");
            await SeedAsync(friend);
            await SeedAsync(MakeFriendship(user, friend));
        }
        var query = new PaginationQuery(2, 5);
        var result = await _usersService.GetFriendsProfileAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        FriendsProfileResponse response = result.Value;
        response.Friends.Should().HaveCount(5);
        response.TotalPages.Should().Be(3);
        
       
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenPageIsOutOfRange()
    {
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        await SeedAsync(user, friend);
        await SeedAsync(MakeFriendship(user, friend));
        var query = new PaginationQuery(10, 5);
        var result = await _usersService.GetFriendsProfileAsync(user.Nickname, query);
        result.IsError.Should().BeFalse();
        FriendsProfileResponse response = result.Value;
        response.Friends.Should().HaveCount(0);
    }
    

}    