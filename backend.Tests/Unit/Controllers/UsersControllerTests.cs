



using backend.Controllers;
using backend.DTO.Users;
using backend.Services.Interfaces;
using backend.Services.Results;
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
    
}

