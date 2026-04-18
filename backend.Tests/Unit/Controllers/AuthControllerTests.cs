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
        ProfileBio: "",
        ProfilePictureUrl: "",
        CreatedAt: DateTime.UtcNow,
        LastActive: DateTime.UtcNow,
        Rating: new RatingResponse(Rapid: 1000, Blitz: 1000, Bullet: 1000)
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
            Password = "Haslo" 
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

}