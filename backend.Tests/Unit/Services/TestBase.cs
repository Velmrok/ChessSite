using backend.Data;
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
}