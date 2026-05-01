using backend.Controllers;
using backend.DTO.Home;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace backend.Tests.Unit.Controllers;

public class HomeControllerTests
{
    private readonly HomeController _controller;
    private readonly IHomeService _homeServiceMock;


    public HomeControllerTests()
    {
        _homeServiceMock = Substitute.For<IHomeService>();
        _controller = new HomeController(_homeServiceMock)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
    [Fact]
    public async Task GetLeaderboard_ShouldReturnOk()
    {
        _homeServiceMock.GetLeaderboardAsync().Returns(new LeaderBoardResponse(
            TopRapidPlayers: [],
            TopBlitzPlayers: [],
            TopBulletPlayers: []
        ));

        var result = await _controller.GetLeaderboard();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}