
using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("/games")]
public class GamesController : MyControllerBase
{

    private readonly IGamesService _gamesService;
    private readonly AppDbContext _dbContext;
    public GamesController(IGamesService gamesService, AppDbContext dbContext)
    {
        _gamesService = gamesService;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetGames([FromQuery] GetGamesQuery query)
    {
        return HandleError(await _gamesService.GetAllGamesAsync(query), Ok);
    }
   
}
