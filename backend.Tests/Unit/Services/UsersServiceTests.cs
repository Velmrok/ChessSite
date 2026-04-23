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

namespace backend.Tests.Unit.Services;

public class UsersServiceTests : TestBase
{
    private readonly UsersService _usersService;
    private readonly IPresenceService _presenceService;

    public UsersServiceTests()
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        _presenceService = Substitute.For<IPresenceService>();
        _usersService = new UsersService(DbContext, cache, _presenceService);
    }

    private User MakeUser(string name, int rapid = 1000, int blitz = 1000, int bullet = 1000,
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

    private async Task SeedAsync(params User[] users)
    {
        DbContext.Users.AddRange(users);
        await DbContext.SaveChangesAsync();
    }


    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers_WhenNoFilters()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"), MakeUser("carol"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());
        var users = result.Value.Response.Users;

        users.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmpty_WhenNoUsersInDb()
    {
        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());

        result.Value.Response.Users.Should().BeEmpty();
        result.Value.Response.TotalPages.Should().Be(0);
    }


    [Fact]
    public async Task GetAllUsersAsync_FiltersBy_Search()
    {
        await SeedAsync(MakeUser("magnus"), MakeUser("hikaru"), MakeUser("maxime"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "ma"));

        result.Value.Response.Users.Should().HaveCount(2);
        result.Value.Response.Users.Should().AllSatisfy(u => u.Nickname.Should().Contain("ma"));
    }

    [Fact]
    public async Task GetAllUsersAsync_Search_IsCaseInsensitive()
    {
        await SeedAsync(MakeUser("Magnus"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "magnus"));

        result.Value.Response.Users.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllUsersAsync_Search_ReturnsEmpty_WhenNoMatch()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "xyz"));

        result.Value.Response.Users.Should().BeEmpty();
    }


    [Fact]
    public async Task GetAllUsersAsync_Limit_ReturnsCorrectCount()
    {
        await SeedAsync(MakeUser("a"), MakeUser("b"), MakeUser("c"), MakeUser("d"), MakeUser("e"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Limit: 2));

        result.Value.Response.Users.Should().HaveCount(2);
        result.Value.Response.TotalPages.Should().Be(3); 
    }

    [Fact]
    public async Task GetAllUsersAsync_Page_ReturnsCorrectPage()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            MakeUser("a", createdAt: now.AddMinutes(-4)),
            MakeUser("b", createdAt: now.AddMinutes(-3)),
            MakeUser("c", createdAt: now.AddMinutes(-2)),
            MakeUser("d", createdAt: now.AddMinutes(-1)),
            MakeUser("e", createdAt: now)
        );

        var page2 = await _usersService.GetAllUsersAsync(new GetUsersQuery(Page: 2, Limit: 2));

        page2.Value.Response.Users.Should().HaveCount(2);
        page2.Value.Response.Users[0].Nickname.Should().Be("c");
        page2.Value.Response.Users[1].Nickname.Should().Be("b");
    }

    [Theory]
    [InlineData(10, 1, 10)]
    [InlineData(10, 2, 5)]
    [InlineData(10, 3, 4)]
    [InlineData(10, 10, 1)]
    [InlineData(10, 11, 1)]
    public async Task GetAllUsersAsync_TotalPages_IsCalculatedCorrectly(int userCount, int limit, int expectedPages)
    {
        var users = Enumerable.Range(1, userCount)
            .Select(i => MakeUser($"user{i}"))
            .ToArray();
        await SeedAsync(users);

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Limit: limit));

        result.Value.Response.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_Nickname_Ascending()
    {
        await SeedAsync(MakeUser("charlie"), MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Nickname, SortDescending: false));

        result.Value.Response.Users.Select(u => u.Nickname).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_Nickname_Descending()
    {
        await SeedAsync(MakeUser("charlie"), MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Nickname, SortDescending: true));

        result.Value.Response.Users.Select(u => u.Nickname).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_CreatedAt_Descending_IsDefault()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            MakeUser("oldest", createdAt: now.AddDays(-2)),
            MakeUser("newest", createdAt: now),
            MakeUser("middle", createdAt: now.AddDays(-1))
        );

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());

        result.Value.Response.Users[0].Nickname.Should().Be("newest");
        result.Value.Response.Users[2].Nickname.Should().Be("oldest");
    }
    [Fact]
    public async Task GetAllUsersAsync_SortBy_CreatedAt_Ascending_IsDefault()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            MakeUser("oldest", createdAt: now.AddDays(-2)),
            MakeUser("newest", createdAt: now),
            MakeUser("middle", createdAt: now.AddDays(-1))
        );

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(SortDescending: false));

        result.Value.Response.Users[0].Nickname.Should().Be("oldest");
        result.Value.Response.Users[2].Nickname.Should().Be("newest");
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_LastActive_Descending()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            MakeUser("inactive", lastActive: now.AddDays(-5)),
            MakeUser("active", lastActive: now)
        );

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.LastActive, SortDescending: true));

        result.Value.Response.Users[0].Nickname.Should().Be("active");
        result.Value.Response.Users[1].Nickname.Should().Be("inactive");
    }

    
    [Fact]
    public async Task GetAllUsersAsync_SortBy_LastActive_Ascending()
    {
        var now = DateTime.UtcNow;
        await SeedAsync(
            MakeUser("inactive", lastActive: now.AddDays(-5)),
            MakeUser("active", lastActive: now)
        );

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.LastActive, SortDescending: false));

        result.Value.Response.Users[0].Nickname.Should().Be("inactive");
        result.Value.Response.Users[1].Nickname.Should().Be("active");
    }

  

    [Theory]
    [InlineData(RatingType.Rapid)]
    [InlineData(RatingType.Blitz)]
    [InlineData(RatingType.Bullet)]
    public async Task GetAllUsersAsync_SortBy_Rating_Descending_SortsCorrectly(RatingType ratingType)
    {
        await SeedAsync(
            MakeUser("weak",   rapid: 800,  blitz: 800,  bullet: 800),
            MakeUser("strong", rapid: 2000, blitz: 2000, bullet: 2000),
            MakeUser("medium", rapid: 1200, blitz: 1200, bullet: 1200)
        );

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Rating, SortDescending: true, RatingType: ratingType));

        result.Value.Response.Users[0].Nickname.Should().Be("strong");
        result.Value.Response.Users[1].Nickname.Should().Be("medium");
        result.Value.Response.Users[2].Nickname.Should().Be("weak");
    }
    [Theory]
    [InlineData(RatingType.Rapid)]
    [InlineData(RatingType.Blitz)]
    [InlineData(RatingType.Bullet)]
    public async Task GetAllUsersAsync_SortBy_Rating_Ascending_SortsCorrectly(RatingType ratingType)
    {
        await SeedAsync(
            MakeUser("weak",   rapid: 800,  blitz: 800,  bullet: 800),
            MakeUser("strong", rapid: 2000, blitz: 2000, bullet: 2000),
            MakeUser("medium", rapid: 1200, blitz: 1200, bullet: 1200)
        );

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Rating, SortDescending: false, RatingType: ratingType));

        result.Value.Response.Users[0].Nickname.Should().Be("weak");
        result.Value.Response.Users[1].Nickname.Should().Be("medium");
        result.Value.Response.Users[2].Nickname.Should().Be("strong");
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_RatingRapid_And_RatingBlitz_AreDifferent()
    {
        await SeedAsync(
            MakeUser("highRapid",  rapid: 2000, blitz: 800),
            MakeUser("highBlitz",  rapid: 800,  blitz: 2000)
        );

        var byRapid = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Rating, SortDescending: true, RatingType: RatingType.Rapid));

        var byBlitz = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Rating, SortDescending: true, RatingType: RatingType.Blitz));

        byRapid.Value.Response.Users[0].Nickname.Should().Be("highRapid");
        byBlitz.Value.Response.Users[0].Nickname.Should().Be("highBlitz");
    }

  

    [Fact]
    public async Task GetAllUsersAsync_TotalPages_ReflectsFilteredCount_NotTotal()
    {
        await SeedAsync(
            MakeUser("magnus"), MakeUser("hikaru"), MakeUser("maxime"),
            MakeUser("other1"), MakeUser("other2"), MakeUser("other3"),
            MakeUser("other4"), MakeUser("other5"), MakeUser("other6"),
            MakeUser("other7"), MakeUser("other8")
        );

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(Search: "ma", Limit: 1));

        result.Value.Response.TotalPages.Should().Be(2); 
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldNotReturnCachedResult_WhenCacheIsEmpty()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"), MakeUser("carol"));

         var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());
        result.Value.IsCached.Should().BeFalse();
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnCachedResult_WhenCacheIsHit()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"), MakeUser("carol"));

         var result1 = await _usersService.GetAllUsersAsync(new GetUsersQuery());
            var result2 = await _usersService.GetAllUsersAsync(new GetUsersQuery());
            result2.Value.IsCached.Should().BeTrue();
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllOnlineUsers_WhenJustOnlineIsTrue()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        var user3 = MakeUser("carol");
        await SeedAsync(user1, user2, user3);

        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>())
            .Returns([user1.Id, user3.Id]);
        
        var response = await _usersService.GetAllUsersAsync(new GetUsersQuery(JustOnline: true));
        response.Value.Response.Users.Should().HaveCount(2);
        response.Value.Response.Users.Select(u => u.Nickname).Should().Contain(["alice", "carol"]);
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnNoUsers_WhenJustOnlineIsTrue_ButNoOneIsOnline()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        await SeedAsync(user1, user2);

        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>())
            .Returns([]);
        var response = await _usersService.GetAllUsersAsync(new GetUsersQuery(JustOnline: true));
        response.Value.Response.Users.Should().BeEmpty();
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers_WhenJustOnlineIsFalse()
    {
        var user1 = MakeUser("alice");
        var user2 = MakeUser("bob");
        await SeedAsync(user1, user2);
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>())
            .Returns([user1.Id]);
        var response = await _usersService.GetAllUsersAsync(new GetUsersQuery(JustOnline: false));
        response.Value.Response.Users.Should().HaveCount(2);

    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldApplyOnlineFilter_BeforePagination()
    {
        var users = Enumerable.Range(1, 20)
            .Select(i => MakeUser($"user{i}"))
            .ToArray();
        await SeedAsync(users);

        var onlineIds = users.Take(5).Select(u => u.Id).ToHashSet();
        _presenceService.GetOnlineIdsAsync(Arg.Any<IEnumerable<Guid>>())
            .Returns(onlineIds);

        var response = await _usersService.GetAllUsersAsync(new GetUsersQuery(Page: 1, Limit: 10, JustOnline: true));
        response.Value.Response.Users.Should().HaveCount(5);
        response.Value.Response.Users.Select(u => u.Nickname).Should().Contain(["user1", "user2", "user3", "user4", "user5"]);
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsersWithinRatingRange_WhenMinMaxRatingIsSet()
    {
        var user1 = MakeUser("alice", rapid: 800);
        var user2 = MakeUser("bob", rapid: 1500);
        var user3 = MakeUser("carol", bullet: 1200, rapid: 2200);
        await SeedAsync(user1, user2, user3);
        var response = await _usersService.GetAllUsersAsync(new GetUsersQuery(
            MinRating: 1000, MaxRating: 2000, RatingType: RatingType.Rapid));
        response.Value.Response.Users.Should().HaveCount(1);
        response.Value.Response.Users[0].Nickname.Should().Be("bob");
    }
}
