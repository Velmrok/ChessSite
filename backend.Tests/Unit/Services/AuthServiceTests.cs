using backend.Controllers;
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace backend.Tests.Unit.Services;


public class AuthServiceTests : TestBase
{
    private readonly AuthService _authService;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AuthServiceTests()
    {
        var jwtMock = Substitute.For<IJwtGenerator>();
        jwtMock.GenerateToken(Arg.Any<User>()).Returns("mocked-jwt-token");
        _authService = new AuthService(DbContext, jwtMock, new PasswordHasher<User>());
        _passwordHasher = new PasswordHasher<User>();

    }
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
         result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().StartWith("mocked-jwt-token");
        var userInDb = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
        userInDb.Should().NotBeNull();
    }
    [Theory]
    [InlineData("test","test","emailTaken","emailTaken")]
    [InlineData("test","loginTaken","test","loginTaken")]
    [InlineData("nicknameTaken","test","test","nicknameTaken")]
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

        var usersCount = await DbContext.Users.CountAsync();
        usersCount.Should().Be(1);
    }
    [Fact]
    public async Task RegisterAsync_ShouldTreatIdentifiersAsCaseInsensitive()
    {
        DbContext.Users.Add(new User
        {
            Nickname = "TestUser",
            Login = "TestUser",
            Email = "TestUser@test.test",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await DbContext.SaveChangesAsync();
        var existingRequest = new RegisterRequest
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "testuser@test.test",
            Password = "Password123!"
        };
        var result = await _authService.RegisterAsync(existingRequest);
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().StartWith("mocked-jwt-token");
    }


    [Fact]
    public async Task LoginAsync_ShouldReturnOK_WhenLoginIsSuccessful()
    {

        DbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await DbContext.SaveChangesAsync();


        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        };
        

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().StartWith("mocked-jwt-token");
    }
    [Fact]
    public async Task LoginAsync_ShouldReturnOK_WhenLoginWithEmailIsSuccessful()
    {

        DbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await DbContext.SaveChangesAsync();


        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Token.Should().StartWith("mocked-jwt-token");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var request = new LoginRequest
        {
            Login = "nonexistentuser",
            Password = "Password123!"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("userNotFound");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login || u.Email == request.Email);
        user.Should().BeNull();
    }
    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        DbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "CorrectPassword")
        });
        await DbContext.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "IncorrectPassword"
        };

        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidCredentials");
        result.Value.Should().BeNull();
    }
    
   
}