
using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend.Controllers;

[ApiController]
[Route("/home")]
public class HomeController : MyControllerBase
{
  
    private readonly IHomeService _homeService;
    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        return HandleError(await _homeService.GetLeaderboardAsync(), Ok);
    }
    
}

