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

namespace backend.Tests.Unit.Services.Home;

public abstract class HomeServiceTestBase : TestBase
{
    protected readonly HomeService _homeService;
    protected readonly IPresenceService _presenceService;

    public HomeServiceTestBase() 
    {
        _presenceService = Substitute.For<IPresenceService>();
        _homeService = new HomeService(_dbContext, _presenceService);
    }
    
}


