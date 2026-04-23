using backend.DTO.Auth;
using backend.Models;
using FluentAssertions;


namespace backend.Tests.Unit.Services.Auith;

public class RefreshTests : AuthTestBase
{
    

   
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

 
}