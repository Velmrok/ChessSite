using System.Data;
using backend.Data;
using backend.DTO.Common;
using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace backend.Tests.Unit.Services.Games;

public abstract class GamesServiceTestBase : TestBase
{
    protected readonly GamesService _gamesService;
    protected readonly IPresenceService _presenceService;

    public GamesServiceTestBase() 
    {
        _presenceService = Substitute.For<IPresenceService>();
        _gamesService = new GamesService(_dbContext, _presenceService);
    }
    
    protected Game MakeGame(User player1, User player2,
    GameType gameType = GameType.Rapid, GameStatus gameStatus = GameStatus.Active,
    User? Winner = null, DateTime? FinishedAt = null) => new()
    {
        WhitePlayerId = player1.Id,
        BlackPlayerId = player2.Id,
        WhitePlayer= player1,
        BlackPlayer = player2,
        Type = gameType,
        Status = gameStatus,
        Winner = Winner,
        FinishedAt = FinishedAt,
        Moves = string.Empty
    };
}


