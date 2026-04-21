using backend.Controllers;
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using backend.Services;
using backend.Services.Helpers.Auth;
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
        var cacheInvalidationServiceMock = Substitute.For<ICacheInvalidationService>();
        var refreshTokenService = new RefreshTokenService(DbContext);
        jwtMock.GenerateToken(Arg.Any<User>()).Returns("mocked-jwt-token");
       
        _authService = new AuthService(DbContext, jwtMock, new PasswordHasher<User>(),cacheInvalidationServiceMock, refreshTokenService);
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

        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
       


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
    public async Task RegisterAsync_ShouldTreatIdentifiersAsCaseSensitive()
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
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");

        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        
    }
    [Fact]
    public async Task LoginAsync_ShouldReturnOK_ShouldTrimInput()
    {
        DbContext.Users.Add(new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        });
        await DbContext.SaveChangesAsync();
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
    public async Task LoginAsync_ShouldReturnValidationError_WhenFieldsAreMissing()
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
        error.Code.Should().Be("invalidLoginOrPassword");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login || u.Email == request.Login);
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
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenFieldIsNullOrEmpty(string? login, string? password)
    {
        var request = new LoginRequest
        {
            Login = login,
            Password = password
        };
        var result = await _authService.LoginAsync(request);
        result.IsError.Should().BeTrue();
    }
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
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

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
 
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnOK_WhenRefreshIsSuccessful()
    {
        var user = new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        var response = await _authService.LoginAsync(new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        });
        response.IsError.Should().BeFalse();
        var refreshToken = response.Value.RefreshToken;

        var result = await _authService.RefreshAsync(refreshToken);
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().StartWith("mocked-jwt-token");
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
       
    
    }
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnUnauthorized_WhenRefreshTokenIsInvalid()
    {
        var result = await _authService.RefreshAsync("invalid-refresh-token");
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidRefreshToken");
    }
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnUnauthorized_WhenRefreshTokenIsMissing()
    {
        var result = await _authService.RefreshAsync("");
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidRefreshToken");
    }
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnUnauthorized_WhenRefreshTokenIsExpired()
    {
        var user = new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@test.test",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = "expired-refresh-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(-1) ,
            User = user
        };
        DbContext.RefreshTokens.Add(refreshTokenEntity);
        await DbContext.SaveChangesAsync();

        var result = await _authService.RefreshAsync(refreshTokenEntity.Token);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidRefreshToken");
    }
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnUnauthorized_WhenRefreshTokenIsRevoked()
    {
        var user = new User
        {
            Nickname = "testuser",
            Login = "testuser",
            Email = "test@test.test",
            PasswordHash = _passwordHasher.HashPassword(null!, "Password123!")
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        var refreshTokenEntity = new RefreshToken
        {
            Token = "revoked-refresh-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsRevoked = true,
            User = user
        };
        DbContext.RefreshTokens.Add(refreshTokenEntity);
        await DbContext.SaveChangesAsync();

        var result = await _authService.RefreshAsync(refreshTokenEntity.Token);
        result.IsError.Should().BeTrue();
        var error = result.FirstError;
        error.Code.Should().Be("invalidRefreshToken");
    }

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
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        var loginResult = await _authService.LoginAsync(new LoginRequest
        {
            Login = "testuser",
            Password = "Password123!"
        });

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