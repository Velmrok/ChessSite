using backend.Controllers;
using backend.DTO.Auth;
using backend.Services.Interfaces;
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

  
    public AuthControllerTests()
    {
        
        _authServiceMock = Substitute.For<IAuthService>();
        
        _controller = new AuthController(_authServiceMock);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

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

        _authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>()).Returns(new AuthResponse("fake-jwt-token"));

        var result = await _controller.Register(request);

        var statusCodeResult = Assert.IsAssignableFrom<IStatusCodeActionResult>(result);
        Assert.Equal(201, statusCodeResult.StatusCode);

    }

}