using backend.DTO.Users;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/admin")]
[Authorize(Roles = "admin")]
public class AdminController : MyControllerBase
{
    private readonly IUsersService _usersService;

    public AdminController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersQuery query)
    {
        return HandleError(await _usersService.GetAllUsersAsync(query), result => Ok(result.Response));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalAccounts = await _usersService.GetCreatedAccountsCountAsync();
        return Ok(new { totalAccounts });
    }
}
