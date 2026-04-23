using backend.DTO.Auth;
using backend.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace backend.Tests.Unit.Services.Auith;

    public class RegisterTests : AuthTestBase
{
    [Fact]
    public async Task RegisterAsync_ShouldReturnOK_WhenRegistrationIsSuccessful()
    {
        var request = new RegisterRequest
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        var result = await _authService.RegisterAsync(request);
        result.IsError.Should().BeFalse();

        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();



        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
        userInDb.Should().NotBeNull();
    }
    [Theory]
    [InlineData("test", "test", "emailTaken", "emailTaken")]
    [InlineData("test", "loginTaken", "test", "loginTaken")]
    [InlineData("nicknameTaken", "test", "test", "nicknameTaken")]
    public async Task RegisterAsync_ShouldReturnConflict_WhenUserAlreadyExists(
        string existingNickname, string existingLogin, string existingEmail, string expectedErrorCode)
    {

        var existingRequest = new RegisterRequest
        {
            Nickname = existingNickname,
            Login = existingLogin,
            Email = existingEmail,
            Password = "Password123!"
        };
        var result = await _authService.RegisterAsync(existingRequest);
        result.IsError.Should().BeFalse();


        var request = new RegisterRequest
        {
            Nickname = expectedErrorCode == "nicknameTaken" ? existingNickname : "newuser",
            Login = expectedErrorCode == "loginTaken" ? existingLogin : "newuser",
            Email = expectedErrorCode == "emailTaken" ? existingEmail : "newuser@example.com",
            Password = "Password123!"
        };

        result = await _authService.RegisterAsync(request);

        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be(expectedErrorCode);

        var usersCount = await _dbContext.Users.CountAsync();
        usersCount.Should().Be(1);
    }
    [Fact]
    public async Task RegisterAsync_ShouldTreatIdentifiersAsCaseSensitive()
    {
        _dbContext.Users.Add(new User
        {
            Nickname = "TestUser",
            Login = "TestUser",
            Email = "TestUser@test.test",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await _dbContext.SaveChangesAsync();
        var existingRequest = new RegisterRequest
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "testuser@test.test",
            Password = "Password123!"
        };
        var result = await _authService.RegisterAsync(existingRequest);
        result.IsError.Should().BeFalse();

        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();

    }
    [Fact]
    public async Task RegisterAsync_ShouldReturnValidationError_WhenFieldsAreMissing()
    {
        var request = new RegisterRequest
        {
            Nickname = "",
            Login = "",
            Email = "",
            Password = ""
        };

        var result = await _authService.RegisterAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidInput");
    }

}