
using System.Net.Http.Json;
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using backend.Tests.Integration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace backend.Tests.Integration.Games;

public class GetGamesTests : TestBase
{

    public GetGamesTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    private void CreateGameAsync(User white, User black, Guid? winnerId = null, DateTime? finishedAt = null,
     GameType type = GameType.Rapid, GameStatus status = GameStatus.Finished)
    {
        var game = new Game
        {
            WhitePlayerId = white.Id,
            BlackPlayerId = black.Id,
            WhitePlayer = white,
            BlackPlayer = black,
            WinnerId = winnerId,
            FinishedAt = finishedAt ?? DateTime.UtcNow,
            Type = type,
            Status = status,
        };
        _dbContext.Games.Add(game);
        
    }
    
        
    [Fact]
    public async Task ShouldReturnOk()
    {
        await LoginAsUserAsync();
        

        var response = await _client.GetAsync("/games");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    [Fact]
    public async Task ShouldReturnGamesData()
    {
        await LoginAsUserAsync();

        var user1 = await MakeUserAsync(nickname: "User1");
        var user2 = await MakeUserAsync(nickname: "User2");
        CreateGameAsync(user1, user2);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync("/games");
        var gamesData = await response.Content.ReadFromJsonAsync<GamesResponse>();
        gamesData.Should().NotBeNull();
        gamesData.Games.Should().HaveCount(1);
        gamesData.Games[0].WhitePlayer.Nickname.Should().Be("User1");
    }
    [Fact]

    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        var response = await _client.GetAsync("/games");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoGames()
    {
        await LoginAsUserAsync();
        
        var response = await _client.GetAsync("/games");
        var gamesData = await response.Content.ReadFromJsonAsync<GamesResponse>();
        gamesData.Should().NotBeNull();
        gamesData.Games.Should().BeEmpty();
    }
}
