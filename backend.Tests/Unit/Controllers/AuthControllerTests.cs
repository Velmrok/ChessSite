using backend.Controllers;
using backend.DTO.Auth;
using backend.DTO.Common;
using backend.Services.Interfaces;
using backend.Services.Results;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;
using Xunit;

namespace backend.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly IAuthService _authServiceMock;
    private readonly AuthController _controller;
    private readonly ICookieService _cookieServiceMock;
  
    public AuthControllerTests()
    {
        
        _authServiceMock = Substitute.For<IAuthService>();
        _cookieServiceMock = Substitute.For<ICookieService>();
        _controller = new AuthController(_authServiceMock, _cookieServiceMock)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
    private static AuthResult FakeAuthResult() => new(
    AccessToken: "fake-jwt-token",
    RefreshToken: "fake-refresh-token",
    User: new GetMeResponse(
        Nickname: "test",
        ProfilePictureUrl: "",
        CreatedAt: DateTime.UtcNow,
        LastActive: DateTime.UtcNow,
        Rating: new RatingStats(Rapid: 1000, Blitz: 1000, Bullet: 1000),
        FriendNicknames: [],
        QueueData: new QueueData(IsInQueue: false)
    )
);

    [Theory]
    [InlineData("emailTaken", "Email is already taken.")]
    [InlineData("loginTaken", "Login is already taken.")]
    [InlineData("nicknameTaken", "Nickname is already taken.")]
    public async Task Register_ShouldReturn409Conflict_WhenUserAlreadyExists(string errorCode, string errorMessage)
    {

        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "test",
            Nickname = "test",
            Password = "123456" 
        };

        var expectedError = Error.Conflict(errorCode, errorMessage);

        _authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>()).Returns(expectedError);


        var result = await _controller.Register(request);

        
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;


        problemDetails.Status.Should().Be(409);
        problemDetails.Title.Should().Be(errorCode);
        problemDetails.Detail.Should().Be(errorMessage);
    }
    [Fact]
    public async Task Register_ShouldReturn201Created_WhenRegistrationIsSuccessful()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "test",
            Nickname = "test",
            Password = "123456"
        };

        _authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>()).Returns(FakeAuthResult());
       

        var result = await _controller.Register(request);
         _cookieServiceMock.Received(1).SetJwtCookie(Arg.Any<HttpResponse>(), Arg.Any<string>());

        var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeActionResult>(result);
        Assert.Equal(201, statusCodeResult.StatusCode);

    }
    [Fact]
    public async Task Register_ShouldReturn401Unauthorized_WhenRegistrationFails()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "test",
            Nickname = "test",
            Password = "123456"
        };

        var expectedError = Error.Unauthorized("registrationFailed", "Registration failed.");
        _authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>()).Returns(expectedError);

        var result = await _controller.Register(request);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(401);
        
    }

    [Fact]
    public async Task Login_ShouldReturn200OK_WhenLoginIsSuccessful()
    {
        var request = new LoginRequest
        {
            Login = "test",
            Password = "123456"
        };
        _authServiceMock.LoginAsync(Arg.Any<LoginRequest>()).Returns(FakeAuthResult());
        var result = await _controller.Login(request);
        _cookieServiceMock.Received(1).SetJwtCookie(Arg.Any<HttpResponse>(), Arg.Any<string>());
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task Login_ShouldReturn401Unauthorized_WhenLoginFails()
    {
        var request = new LoginRequest
        {
            Login = "test",
            Password = "WrongPassword!"
        };
        var expectedError = Error.Unauthorized("invalidCredentials", "Invalid login or password.");
        _authServiceMock.LoginAsync(Arg.Any<LoginRequest>()).Returns(expectedError);
        var result = await _controller.Login(request);
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(401);
        problemDetails.Title.Should().Be("invalidCredentials");
        problemDetails.Detail.Should().Be("Invalid login or password.");
    }

    [Fact]
    public async Task Logout_ShouldReturn204NoContent()
    {
        var result = await _controller.Logout();
        _cookieServiceMock.Received(1).DeleteJwtCookie(Arg.Any<HttpResponse>());
        _cookieServiceMock.Received(1).DeleteRefreshTokenCookie(Arg.Any<HttpResponse>());
        var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeActionResult>(result);
        Assert.Equal(204, statusCodeResult.StatusCode);
    }
    [Fact]
    public async Task Refresh_ShouldReturn200OK_WhenRefreshIsSuccessful()
    {
        
        _authServiceMock.RefreshAsync(Arg.Any<string>()).Returns(FakeAuthResult());
        var result = await _controller.Refresh();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _cookieServiceMock.Received(1).SetJwtCookie(Arg.Any<HttpResponse>(), Arg.Any<string>());
    }
    [Fact]
    public async Task Refresh_ShouldReturn401Unauthorized_WhenRefreshFails()
    {
        var expectedError = Error.Unauthorized("invalidRefreshToken", "Refresh token is invalid or has expired.");
        _authServiceMock.RefreshAsync(Arg.Any<string>()).Returns(expectedError);
        var result = await _controller.Refresh();
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(401);
        problemDetails.Title.Should().Be("invalidRefreshToken");
        problemDetails.Detail.Should().Be("Refresh token is invalid or has expired.");
    }
    [Fact]
    public async Task GetMe_ShouldReturn200OK_WhenAccessTokenIsValid()
    {
        var expectedResponse = new GetMeResponse(
            Nickname: "test",
            ProfilePictureUrl: "",
            CreatedAt: DateTime.UtcNow,
            LastActive: DateTime.UtcNow,
            Rating: new RatingStats(Rapid: 1000, Blitz: 1000, Bullet: 1000),
            FriendNicknames: [],
            QueueData: new QueueData(IsInQueue: false)
        );
        _authServiceMock.GetMeAsync(Arg.Any<string>()).Returns(expectedResponse);
        var result = await _controller.GetMe();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var response = Assert.IsType<GetMeResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Nickname, response.Nickname);
    }
    [Fact]
    public async Task GetMe_ShouldReturn401Unauthorized_WhenAccessTokenIsInvalid()
    {
        var expectedError = Error.Unauthorized("invalidAccessToken", "Access token is invalid.");
        _authServiceMock.GetMeAsync(Arg.Any<string>()).Returns(expectedError);
        var result = await _controller.GetMe();
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(401);
        problemDetails.Title.Should().Be("invalidAccessToken");
        problemDetails.Detail.Should().Be("Access token is invalid.");
    }
}