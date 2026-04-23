
using System.Linq.Expressions;
using System.Text.Json;
using backend.Data;
using backend.DTO.Users;
using backend.Enums;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using backend.Services.Results;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IPresenceService _presenceService;
    public UsersService(AppDbContext context, IDistributedCache cache, IPresenceService presenceService)
    {
        _context = context;
        _cache = cache;
        _presenceService = presenceService;
    }

    private static IQueryable<User> ApplySort(IQueryable<User> users, UsersSortBy sortBy, RatingType ratingType, bool descending)
    {
        if (sortBy == UsersSortBy.Rating)
            return ratingType switch
            {
                RatingType.Blitz => users.OrderByField(u => u.Rating.Blitz, descending),
                RatingType.Bullet => users.OrderByField(u => u.Rating.Bullet, descending),
                _ => users.OrderByField(u => u.Rating.Rapid, descending),
            };

        return sortBy switch
        {
            UsersSortBy.Nickname => users.OrderByField(u => u.Nickname, descending),
            UsersSortBy.LastActive => users.OrderByField(u => u.LastActive, descending),
            //UsersSortBy.OnlineStatus => users.OrderByField(u => u.OnlineStatus, descending),
            _ => users.OrderByField(u => u.CreatedAt, descending),
        };
    }
    public async Task<ErrorOr<UsersResult>> GetAllUsersAsync(GetUsersQuery query)
    {
       
        var version = await _cache.GetStringAsync("users:version") ?? "0";
        var cacheKey = $"users_v{version}:users:{query.Page}:{query.Limit}:{query.Search}:" +
                  $"{query.SortBy}:{query.SortDescending}:{query.RatingType}:" +
                  $"{query.MinRating}:{query.MaxRating}:{query.JustOnline}";

        
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<UsersResponse>(cached)!;
            return new UsersResult(cachedResponse, true);
        }

        var users = _context.Users.AsQueryable();
        

        if (!string.IsNullOrEmpty(query.Search))
        {
            users = users.Where(u => u.Nickname.ToLower().Contains(query.Search.ToLower()));
        }
        users = query.RatingType switch 
        {
            RatingType.Blitz => users.Where(u => u.Rating.Blitz >= query.MinRating && u.Rating.Blitz <= query.MaxRating),
            RatingType.Bullet => users.Where(u => u.Rating.Bullet >= query.MinRating && u.Rating.Bullet <= query.MaxRating),
            _ => users.Where(u => u.Rating.Rapid >= query.MinRating && u.Rating.Rapid <= query.MaxRating),
        };
        
        
       
        if (query.JustOnline)
        {
            var onlineIds = await _presenceService.GetOnlineIdsAsync(users.Select(u => u.Id));
            users = users.Where(u => onlineIds.Contains(u.Id));
        }

        users = ApplySort(users, query.SortBy, query.RatingType, query.SortDescending);

        var result = await users
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(u => u.ToUserResponse())
            .ToListAsync();
        var totalCount = await users.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

        var response = new UsersResponse(result, totalPages);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options);

        return new UsersResult(response, false);
    }

    public async Task<ErrorOr<UserProfileResponse>> GetUserProfileAsync(string nickname)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        var isOnline = await _presenceService.IsOnlineAsync(user.Id);
        var response = user.ToUserProfileResponse(isOnline);
        return response;
    }
    public async Task<ErrorOr<FriendsResponse>> GetFriendsAsync(string nickname, PaginationQuery pagination)
    {

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }

        var friends = _context.Friendships
            .Where(f => f.User.Nickname == nickname)
            .Select(f => f.Friend.ToUserResponse())
            .Skip((pagination.PageNumber - 1) * pagination.Limit)
            .Take(pagination.Limit);

        var response = new FriendsResponse(
            await friends.ToListAsync(),
            (int)Math.Ceiling(await friends.CountAsync() / (double)pagination.Limit)
        );
        return response;
    }
    public async Task<ErrorOr<Success>> AddFriendAsync(AddFriendRequest request, string currentUserNickname)
    {
        if (request.Nickname == currentUserNickname)
        {
            return Error.Validation("sameUser", "You cannot add yourself as a friend.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == currentUserNickname);
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == request.Nickname);

        if (user == null || friend == null)
        {
            return Error.NotFound("userNotFound", "One or both users were not found.");
        }

        var existingFriendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == friend.Id);

        if (existingFriendship != null)
        {
            return Error.Conflict("alreadyFriends", "These users are already friends.");
        }

        var friendship = new Friendship
        {
            UserId = user.Id,
            FriendId = friend.Id
        };
        var reverseFriendship = new Friendship
        {
            UserId = friend.Id,
            FriendId = user.Id
        };

        _context.Friendships.Add(friendship);
        _context.Friendships.Add(reverseFriendship);
        await _context.SaveChangesAsync();

        return Result.Success;

    }
}
