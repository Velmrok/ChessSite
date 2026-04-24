using backend.DTO.Auth;
using backend.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace backend.Tests.Unit.Services.Auth;

public class LoginTests : AuthTestBase
{

    [Fact]
    public async Task ShouldReturnOK_WhenLoginIsSuccessful()
    {

        _dbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await _dbContext.SaveChangesAsync();


        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        };


        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();

    }
    [Fact]
    public async Task ShouldReturnOK_ShouldTrimInput()
    {
        _dbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await _dbContext.SaveChangesAsync();
        var existingRequest = new LoginRequest
        {
            Login = "testuser   ",
            Password = "Password123!            "
        };
        var result = await _authService.LoginAsync(existingRequest);
        result.IsError.Should().BeFalse();

        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();

    }
    [Fact]
    public async Task ShouldReturnValidationError_WhenFieldsAreMissing()
    {
        var request = new LoginRequest
        {
            Login = "",
            Password = ""
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidLoginOrPassword");
    }

    [Fact]
    public async Task ShouldReturnOK_WhenLoginWithEmailIsSuccessful()
    {

        _dbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await _dbContext.SaveChangesAsync();


        var request = new LoginRequest
        {
            Login = "test@example.com",
            Password = "Password123!"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();

    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var request = new LoginRequest
        {
            Login = "nonexistentuser",
            Password = "Password123!"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidLoginOrPassword");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login || u.Email == request.Login);
        user.Should().BeNull();
    }
    [Fact]
    public async Task ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        _dbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "CorrectPassword")
        });
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "IncorrectPassword"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidLoginOrPassword");
        result.Value.Should().BeNull();
    }
    [Theory]
    [InlineData(null, "Password123!")]
    [InlineData("testuser", null)]
    [InlineData("", "Password123!")]
    [InlineData("testuser", "")]
    [InlineData(null, null)]
    [InlineData("", "")]
    public async Task ShouldReturnUnauthorized_WhenFieldIsNullOrEmpty(string? login, string? password)
    {
        var request = new LoginRequest
        {
            Login = login!,
            Password = password!
        };
        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
    }
}