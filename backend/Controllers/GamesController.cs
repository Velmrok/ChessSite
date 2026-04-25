
using backend.Data;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend.Controllers;

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
    [HttpPost]
    public async Task<IActionResult> AddGame([FromBody] AddGameRequest request)
    {
        var whitePlayer = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == request.WhitePlayerNickname);
        var blackPlayer = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == request.BlackPlayerNickname);
        _dbContext.Games.Add(new Game
        {
            WhitePlayerId = whitePlayer.Id,
            BlackPlayerId = blackPlayer.Id,
            Type = request.Type,
            Time = request.Time,
            Increment = request.Increment,
        });
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}
public class AddGameRequest
{
    public string WhitePlayerNickname { get; set; }
    public string BlackPlayerNickname { get; set; }
    public GameType Type { get; set; }
    public int Time { get; set; }
    public int Increment { get; set; }
}

