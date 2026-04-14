using backend.Controllers;
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace backend.Tests.Unit.Services;


public class AuthServiceTests
{
    private readonly AuthService _authService;
    
    public AuthServiceTests()
    {
       var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContextMock = new AppDbContext(options);
        var jwtMock = Substitute.For<IJwtGenerator>();
        _authService = new AuthService(dbContextMock, jwtMock);

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
    }
    [Theory]
    [InlineData("test","test","emailTaken","emailTaken")]
    [InlineData("test","loginTaken","test","loginTaken")]
    [InlineData("nicknameTaken","test","test","nicknameTaken")]
    public async Task RegisterAsync_ShouldReturnConflict_WhenUserAlreadyExists(
        string existingNickname, string existingLogin, string existingEmail, string expectedErrorCode)
    {
        var existingUser = new User
        {
            Nickname = existingNickname,
            Login = existingLogin,
            Email = existingEmail,
            PasswordHash = "hashedpassword",
        };

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
    }
}