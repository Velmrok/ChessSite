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
    
}


