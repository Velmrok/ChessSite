using backend.Data;
using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace backend.Tests.Unit.Services;

public class UsersServiceTests : TestBase
{
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        _usersService = new UsersService(DbContext, cache);
    }

    private User MakeUser(string name, int rapid = 1000, int blitz = 1000, int bullet = 1000,
        DateTime? createdAt = null, DateTime? lastActive = null) => new()
    {
        Nickname = name,
        Login = name,
        Email = $"{name}@test.com",
        PasswordHash = "hash",
        RapidRating = rapid,
        BlitzRating = blitz,
        BulletRating = bullet,
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
        var users = result.Response.Users;

        users.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmpty_WhenNoUsersInDb()
    {
        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());

        result.Response.Users.Should().BeEmpty();
        result.Response.TotalPages.Should().Be(0);
    }


    [Fact]
    public async Task GetAllUsersAsync_FiltersBy_Search()
    {
        await SeedAsync(MakeUser("magnus"), MakeUser("hikaru"), MakeUser("maxime"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "ma"));

        result.Response.Users.Should().HaveCount(2);
        result.Response.Users.Should().AllSatisfy(u => u.Nickname.Should().Contain("ma"));
    }

    [Fact]
    public async Task GetAllUsersAsync_Search_IsCaseInsensitive()
    {
        await SeedAsync(MakeUser("Magnus"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "magnus"));

        result.Response.Users.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllUsersAsync_Search_ReturnsEmpty_WhenNoMatch()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Search: "xyz"));

        result.Response.Users.Should().BeEmpty();
    }


    [Fact]
    public async Task GetAllUsersAsync_Limit_ReturnsCorrectCount()
    {
        await SeedAsync(MakeUser("a"), MakeUser("b"), MakeUser("c"), MakeUser("d"), MakeUser("e"));

        var result = await _usersService.GetAllUsersAsync(new GetUsersQuery(Limit: 2));

        result.Response.Users.Should().HaveCount(2);
        result.Response.TotalPages.Should().Be(3); 
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

        page2.Response.Users.Should().HaveCount(2);
        page2.Response.Users[0].Nickname.Should().Be("c");
        page2.Response.Users[1].Nickname.Should().Be("b");
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

        result.Response.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_Nickname_Ascending()
    {
        await SeedAsync(MakeUser("charlie"), MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Nickname, SortDescending: false));

        result.Response.Users.Select(u => u.Nickname).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllUsersAsync_SortBy_Nickname_Descending()
    {
        await SeedAsync(MakeUser("charlie"), MakeUser("alice"), MakeUser("bob"));

        var result = await _usersService.GetAllUsersAsync(
            new GetUsersQuery(SortBy: UsersSortBy.Nickname, SortDescending: true));

        result.Response.Users.Select(u => u.Nickname).Should().BeInDescendingOrder();
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

        result.Response.Users[0].Nickname.Should().Be("newest");
        result.Response.Users[2].Nickname.Should().Be("oldest");
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

        result.Response.Users[0].Nickname.Should().Be("oldest");
        result.Response.Users[2].Nickname.Should().Be("newest");
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

        result.Response.Users[0].Nickname.Should().Be("active");
        result.Response.Users[1].Nickname.Should().Be("inactive");
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

        result.Response.Users[0].Nickname.Should().Be("inactive");
        result.Response.Users[1].Nickname.Should().Be("active");
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

        result.Response.Users[0].Nickname.Should().Be("strong");
        result.Response.Users[1].Nickname.Should().Be("medium");
        result.Response.Users[2].Nickname.Should().Be("weak");
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

        result.Response.Users[0].Nickname.Should().Be("weak");
        result.Response.Users[1].Nickname.Should().Be("medium");
        result.Response.Users[2].Nickname.Should().Be("strong");
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

        byRapid.Response.Users[0].Nickname.Should().Be("highRapid");
        byBlitz.Response.Users[0].Nickname.Should().Be("highBlitz");
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

        result.Response.TotalPages.Should().Be(2); 
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldNotReturnCachedResult_WhenCacheIsEmpty()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"), MakeUser("carol"));

         var result = await _usersService.GetAllUsersAsync(new GetUsersQuery());
        result.IsCached.Should().BeFalse();
    }
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnCachedResult_WhenCacheIsHit()
    {
        await SeedAsync(MakeUser("alice"), MakeUser("bob"), MakeUser("carol"));

         var result1 = await _usersService.GetAllUsersAsync(new GetUsersQuery());
            var result2 = await _usersService.GetAllUsersAsync(new GetUsersQuery());
            result2.IsCached.Should().BeTrue();
    }
}
