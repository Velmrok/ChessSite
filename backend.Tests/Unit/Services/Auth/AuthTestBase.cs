using backend.Models;
using backend.Services;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace backend.Tests.Unit.Services.Auth
{
    public abstract class AuthTestBase : TestBase
    {
        protected readonly AuthService _authService;
        protected readonly IPasswordHasher<User> _passwordHasher;
        public AuthTestBase() 
        {
            var jwtMock = Substitute.For<IJwtGenerator>();
            var cacheInvalidationServiceMock = Substitute.For<ICacheInvalidationService>();
            var refreshTokenService = new RefreshTokenService(_dbContext);
            jwtMock.GenerateToken(Arg.Any<User>()).Returns("mocked-jwt-token");

            _authService = new AuthService(_dbContext, jwtMock, new PasswordHasher<User>(), cacheInvalidationServiceMock, refreshTokenService);
            _passwordHasher = new PasswordHasher<User>();

        }
    }
}