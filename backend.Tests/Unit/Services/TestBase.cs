using backend.Data;
using Microsoft.EntityFrameworkCore;
namespace backend.Tests.Unit.Services;
public abstract class TestBase
{
    protected readonly AppDbContext DbContext;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        DbContext = new AppDbContext(options);
    }
}