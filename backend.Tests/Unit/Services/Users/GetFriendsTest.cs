using backend.DTO.Users;
using backend.Enums;
using FluentAssertions;
using NSubstitute;

namespace backend.Tests.Unit.Services.Users;

public class GetFriendsTests : UsersServiceTestBase
{
    
    [Fact]
    public async Task GetFriendsAsync_ShouldReturnOK_WhenUserHasFriends()
    {
       
        var user = MakeUser("alice");
        var friend = MakeUser("bob");
        
}    