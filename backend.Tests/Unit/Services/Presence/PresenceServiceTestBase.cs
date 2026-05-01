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
using StackExchange.Redis;

namespace backend.Tests.Unit.Services.Presence;

public abstract class PresenceServiceTestBase : TestBase
{
    protected readonly PresenceService _presenceService;
    protected readonly IConnectionMultiplexer _redis;
    protected readonly IDatabase _db;
    protected const string OnlineSetKey = "users:online";
    public PresenceServiceTestBase() 
    {
        _redis = Substitute.For<IConnectionMultiplexer>();
        _db = Substitute.For<IDatabase>();
        _redis.GetDatabase().Returns(_db);
       
        _presenceService = new PresenceService(_redis, _dbContext);
       
    }
    
}


