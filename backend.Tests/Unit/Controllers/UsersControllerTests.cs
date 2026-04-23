



using backend.Controllers;
using backend.DTO.Users;
using backend.Services.Interfaces;
using backend.Services.Results;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace backend.Tests.Unit.Controllers;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly IUsersService _usersServiceMock;
    private readonly IDistributedCache _cacheMock;

    public UsersControllerTests()
    {
        _usersServiceMock = Substitute.For<IUsersService>();
        _cacheMock = Substitute.For<IDistributedCache>();
        _controller = new UsersController(_usersServiceMock, _cacheMock)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
    [Fact]
    public async Task GetUsers_ShouldReturnOk()
    {
        var query = new GetUsersQuery();

        _usersServiceMock.GetAllUsersAsync(query).Returns(new UsersResult
        (
            new UsersResponse([],0),
            false
        ));

        var result = await _controller.GetUsers(query);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task GetUsers_ShouldReturnUsers()
    {
        var query = new GetUsersQuery();

        _usersServiceMock.GetAllUsersAsync(query).Returns(new UsersResult
        (
            new UsersResponse([],0),
            false
        ));

        var result = await _controller.GetUsers(query);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task GetUsers_ShouldReturn400_WhenInvalidParametersProvided()
    {
        var query = new GetUsersQuery { Limit = -1 };

        var expectedError = Error.Validation("Invalid parameters", "Limit must be greater than 0");

        _usersServiceMock.GetAllUsersAsync(query).Returns(expectedError);

        var result = await _controller.GetUsers(query);

       
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;

         var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        objectResult.Value.Should().BeOfType<ProblemDetails>();

        problemDetails.Status.Should().Be(400);
        problemDetails.Title.Should().Be(expectedError.Code);
        problemDetails.Detail.Should().Be(expectedError.Description);

           
    }
    [Fact]
    public async Task GetFriends_ShouldReturnOk()
    {
        var nickname = "testuser";
        var query = new PaginationQuery { PageNumber = 1, Limit = 10 };

        _usersServiceMock.GetFriendsAsync(nickname, query).Returns(new FriendsResponse
        (
            [],1
        ));

        var result = await _controller.GetFriends(nickname, query);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task AddFriend_ShouldReturnOk()
    {
        var nickname = "testuser";
        var request = new AddFriendRequest { Nickname = "frienduser" };

        _usersServiceMock.AddFriendAsync(request, nickname).Returns(new Success());

        var result = await _controller.AddFriend(request);

        var okResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, okResult.StatusCode);
    }
    [Fact]
    public async Task UpdateUserBio_ShouldReturnOk()
    {
        var nickname = "testuser";
        var request = new UpdateUserBioRequest ( Bio : "This is my new bio." );
        _usersServiceMock.UpdateUserBioAsync(nickname, request)
        .Returns(new UpdateUserBioResponse ( Bio : request.Bio ));
        var result = await _controller.UpdateUserBio(nickname, request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}

