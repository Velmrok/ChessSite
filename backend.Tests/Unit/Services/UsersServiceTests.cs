
using backend.Data;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Unit.Services;


public class UsersServiceTests : TestBase
{
    private readonly UsersService _usersService;
    
    public UsersServiceTests()
    {
        _usersService = new UsersService(DbContext);

    }
    [Fact]
    public async Task GetAllUsersAsync_ReturnsListOfUsers()
    {
        
        var user1 = new User { Nickname = "user1", Login = "user1", Email = "user1@example.com",PasswordHash = "hashedpassword" };
        var user2 = new User { Nickname = "user2", Login = "user2", Email = "user2@example.com",PasswordHash = "hashedpassword" };

        DbContext.Users.Add(user1);
        DbContext.Users.Add(user2);
        await DbContext.SaveChangesAsync();

        var result = await _usersService.GetAllUsersAsync();


        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Nickname.Should().Be("user1");
        result[1].Nickname.Should().Be("user2");
    }
}