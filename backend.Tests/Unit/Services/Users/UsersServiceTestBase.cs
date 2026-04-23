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

namespace backend.Tests.Unit.Services.Users;

public abstract class UsersServiceTestBase : TestBase
{
    protected readonly UsersService _usersService;
    protected readonly IPresenceService _presenceService;

    public UsersServiceTestBase()
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        _presenceService = Substitute.For<IPresenceService>();
        _usersService = new UsersService(_dbContext, cache, _presenceService);
    }
    protected User MakeUser(string name, int rapid = 1000, int blitz = 1000, int bullet = 1000,
        DateTime? createdAt = null, DateTime? lastActive = null) => new()
    {
        Nickname = name,
        Login = name,
        Email = $"{name}@test.com",
        PasswordHash = "hash",
        Rating = new RatingStats(rapid, blitz, bullet),
        CreatedAt = createdAt ?? DateTime.UtcNow,
        LastActive = lastActive ?? DateTime.UtcNow,
    };

    protected async Task SeedAsync(params User[] users)
    {
        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();
    }
}


