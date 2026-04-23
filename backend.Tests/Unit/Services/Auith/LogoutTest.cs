using backend.DTO.Auth;
using backend.Models;
using FluentAssertions;


namespace backend.Tests.Unit.Services.Auith;

public class LogoutTests : AuthTestBase
{
    

    [Fact]
    public async Task LogoutAsync_ShouldReturnOK_WhenLogoutIsSuccessful()
    {
        var user = new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _authService.LoginAsync(new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        });
        response.IsError.Should().BeFalse();
        var refreshToken = response.Value.RefreshToken;

        var result = await _authService.LogoutAsync(refreshToken);
        result.IsError.Should().BeFalse();
    }
 
}