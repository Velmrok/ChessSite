
using backend.DTO.Games;
using backend.Enums;
using backend.Models;
using FluentAssertions;

namespace backend.Tests.Unit.Services.Games;

public class GetAllGamesTests : GamesServiceTestBase
{
    
    public GetAllGamesTests() : base()
    {
    }
    [Fact]
    public async Task ShouldReturnAllGamesForUser()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        var user3 = MakeUser("charlie");
        await SeedAsync(user1, user2, user3);

        var game1 = MakeGame(user1, user2, GameType.Rapid, GameStatus.Active);
        var game2 = MakeGame(user1, user2, GameType.Blitz, GameStatus.Finished, Winner: user1, FinishedAt: DateTime.UtcNow);
        var game3 = MakeGame(user1, user3, GameType.Bullet, GameStatus.Active);

        await SeedAsync(game1, game2, game3);

        var query = new GetGamesQuery(Query : "bob");
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(2);
        result.Value.Games.Should().ContainSingle(g => g.Id == game1.Id.ToString());
        result.Value.Games.Should().ContainSingle(g => g.Id == game2.Id.ToString());
    }
    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoGamesMatchQuery()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        await SeedAsync(user1, user2);
        var game1 = MakeGame(user1, user2);
        await SeedAsync(game1);
        var query = new GetGamesQuery(Query : "nonexistentuser");
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(0);
    }
    [Fact]
    public async Task ShouldReturnAllGames_WhenQueryIsNull()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        var user3 = MakeUser("charlie");
        await SeedAsync(user1, user2, user3);  
        var game1 = MakeGame(user1, user2);
        var game2 = MakeGame(user1, user3);
        await SeedAsync(game1, game2);
        var query = new GetGamesQuery(Query : null);
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(2);
    }
    [Fact]
    public async Task ShouldReturnAllGames_WhenQueryIsEmpty()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        await SeedAsync(user1, user2);
        var game1 = MakeGame(user1, user2);
        await SeedAsync(game1);
        var query = new GetGamesQuery(Query : "");
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(1);
    }
    [Fact]
    public async Task ShouldReturnPaginatedGames()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        await SeedAsync(user1, user2);
        var games = new List<Game>();
        for (int i = 0; i < 25; i++)        {
            var game = MakeGame(user1, user2,  FinishedAt: DateTime.UtcNow.AddMinutes(-i));
            games.Add(game);
        }
        await SeedAsync<Game>(games);
        var query = new GetGamesQuery(PageNumber: 2, Limit: 10);
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(10);
        result.Value.TotalPages.Should().Be(3);
        result.Value.Games.Should().ContainSingle(g => g.Id == games[10].Id.ToString());
    }
    [Fact]
    public async Task ShouldReturnFilteredGamesByType()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");    
        await SeedAsync(user1, user2);
        var game1 = MakeGame(user1, user2, GameType.Rapid);
        var game2 = MakeGame(user1, user2, GameType.Blitz);
        await SeedAsync(game1, game2);
        var query = new GetGamesQuery(GameType: GameType.Rapid);
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(1);
        result.Value.Games.Should().ContainSingle(g => g.Id == game1.Id.ToString());

        var query2 = new GetGamesQuery(GameType: GameType.Blitz);
        var result2 = await _gamesService.GetAllGamesAsync(query2);
        result2.IsError.Should().BeFalse();
        result2.Value.Games.Should().HaveCount(1);
        result2.Value.Games.Should().ContainSingle(g => g.Id == game2.Id.ToString());
    }
    [Fact]
    public async Task ShouldReturnFilteredGamesByStatus()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");    
        await SeedAsync(user1, user2);
        var game1 = MakeGame(user1, user2, gameStatus: GameStatus.Active);
        var game2 = MakeGame(user1, user2, gameStatus: GameStatus.Finished);
        await SeedAsync(game1, game2);
        var query = new GetGamesQuery(GameStatus: GameStatus.Active);
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(1);
        result.Value.Games.Should().ContainSingle(g => g.Id == game1.Id.ToString());
    }
    [Fact]
    public async Task ShouldReturnSortedGames()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");    
        await SeedAsync(user1, user2);
        var game1 = MakeGame(user1, user2, GameType.Rapid, FinishedAt: DateTime.UtcNow.AddMinutes(-10));
        var game2 = MakeGame(user1, user2, GameType.Rapid, FinishedAt: DateTime.UtcNow);
        await SeedAsync(game1, game2);

        var query = new GetGamesQuery(SortBy: GamesSortBy.FinishedAt, SortDescending: false);
        var result = await _gamesService.GetAllGamesAsync(query);
        result.IsError.Should().BeFalse();
        result.Value.Games.Should().HaveCount(2);
        result.Value.Games.First().Id.Should().Be(game1.Id.ToString());

        var query2 = new GetGamesQuery(SortBy: GamesSortBy.FinishedAt, SortDescending: true);
        var result2 = await _gamesService.GetAllGamesAsync(query2);
        result2.IsError.Should().BeFalse();
        result2.Value.Games.Should().HaveCount(2);
        result2.Value.Games.First().Id.Should().Be(game2.Id.ToString());

    }
}