using backend.DTO.Auth;
using backend.Models;
using FluentAssertions;

namespace backend.Tests.Unit.Services.Auth;

public class GetMeTests : AuthTestBase
{
    [Fact]
    public async Task GetMeAsync_ShouldReturnOK_WhenUserExists()
    {
        var user = new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@test.test",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var loginResult = await _authService.LoginAsync(new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        });
        loginResult.IsError.Should().BeFalse();

        var result = await _authService.GetMeAsync(user.Id.ToString());
        result.IsError.Should().BeFalse();
    }
    [Fact]
    public async Task GetMeAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var result = await _authService.GetMeAsync(Guid.NewGuid().ToString());
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("userNotFound");
    }

}