using backend.Data;
using backend.DTO.Common;
using backend.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace backend.Tests.Unit.Services;
public abstract class TestBase
{
    protected readonly AppDbContext _dbContext;

    protected TestBase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

    }
    protected User MakeUser(string name, int rapid = 1000, int blitz = 1000, int bullet = 1000,
        DateTime? createdAt = null, DateTime? lastActive = null,string bio = "") => new()
    {
        Nickname = name,
        Login = name,
        Email = $"{name}@test.com",
        PasswordHash = "hash",
        Rating = new RatingStats(rapid, blitz, bullet),
        CreatedAt = createdAt ?? DateTime.UtcNow,
        LastActive = lastActive ?? DateTime.UtcNow,
        ProfileBio = bio,
        ProfilePictureUrl = string.Empty
    };

    protected Friendship MakeFriendship(User user, User friend) => new()
    {
        UserId = user.Id,
        FriendId = friend.Id,
        User = user,
        Friend = friend,
    };



    protected async Task SeedAsync<T>(params T[] entities) where T : class
    {
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }
    protected async Task SeedAsync<T>(IEnumerable<T> entities) where T : class
{
    _dbContext.Set<T>().AddRange(entities);
    await _dbContext.SaveChangesAsync();
}
}