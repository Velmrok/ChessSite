



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
            new UsersSearchResponse([],0),
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
            new UsersSearchResponse([],0),
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
    public async Task GetFriendsProfile_ShouldReturnOk()
    {
        var nickname = "testuser";
        var query = new PaginationQuery { PageNumber = 1, Limit = 10 };

        _usersServiceMock.GetFriendsProfileAsync(nickname, query).Returns(new FriendsProfileResponse
        (
            [],1
        ));

        var result = await _controller.GetFriendsProfile(nickname, query);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task AddFriend_ShouldReturnOk()
    {
        var nickname = "testuser";
      

        _usersServiceMock.AddFriendAsync("friendUser", nickname).Returns(new Success());

        var result = await _controller.AddFriend(nickname);

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
        var result = await _controller.UpdateUserBio(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task RemoveFriend_ShouldReturnOk()
    {
        var nickname = "testuser";

        _usersServiceMock.RemoveFriendAsync("friendUser", nickname).Returns(new Success());

        var result = await _controller.RemoveFriend(nickname);

        var okResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, okResult.StatusCode);
    }
    [Fact]
    public async Task GetUserProfile_ShouldReturnOk()
    {
        var nickname = "testuser";

        _usersServiceMock.GetUserProfileAsync(nickname).Returns(new UserProfileResponse
        (
            Nickname: nickname,
            Bio: "This is my bio.",
            ProfilePictureUrl: "http://example.com/profile.jpg",
            IsOnline: true,
            UserInfo: new UserInfo
            (
                Rating: new DTO.Common.RatingStats(Rapid: 1200, Blitz: 1300, Bullet: 1100),
                CreatedAt: DateTime.UtcNow.AddYears(-1).ToString("O"),
                GamesPlayed: 100,
                Wins: new DTO.Common.RatingStats(Rapid: 60, Blitz: 70, Bullet: 50),
                Losses: new DTO.Common.RatingStats(Rapid: 30, Blitz: 20, Bullet: 40),
                Draws: new DTO.Common.RatingStats(Rapid: 10, Blitz: 10, Bullet: 10),
                TotalWins: 180,
                TotalLosses: 90,
                TotalDraws: 30
               
            )
        ));

        var result = await _controller.GetUserProfile(nickname);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task GetOnlineFriends_ShouldReturnOk()
    {
        var nickname = "testuser";
        var query = new PaginationQuery { PageNumber = 1, Limit = 10 };

        _usersServiceMock.GetOnlineFriendsAsync(nickname, query).Returns(new OnlineFriendsResponse
        (
            [],1
        ));

        var result = await _controller.GetOnlineFriends(query);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}

