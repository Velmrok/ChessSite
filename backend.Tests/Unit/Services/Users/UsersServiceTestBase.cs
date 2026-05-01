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

namespace backend.Tests.Unit.Services.Users;

public abstract class UsersServiceTestBase : TestBase
{
    protected readonly UsersService _usersService;
    protected readonly IPresenceService _presenceService;
    protected readonly IStorageService _storageService;

    public UsersServiceTestBase() 
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        _presenceService = Substitute.For<IPresenceService>();
        _storageService = Substitute.For<IStorageService>();

        _usersService = new UsersService(_dbContext, cache, _presenceService, _storageService);
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


