
using System.Linq.Expressions;
using System.Text.Json;
using backend.Data;
using backend.DTO.Common;
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
    private readonly AppDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly IPresenceService _presenceService;
    private readonly IStorageService _storageService;
    public UsersService(AppDbContext context, IDistributedCache cache, IPresenceService presenceService, IStorageService storageService)
    {
        _dbContext = context;
        _cache = cache;
        _presenceService = presenceService;
        _storageService = storageService;
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
            var cachedResponse = JsonSerializer.Deserialize<UsersSearchResponse>(cached)!;
            return new UsersResult(cachedResponse, true);
        }

        var users = _dbContext.Users.AsQueryable();


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


        var onlineIds = await _presenceService.GetOnlineIdsAsync(users.Select(u => u.Id));
        if (query.JustOnline)
        {

            users = users.Where(u => onlineIds.Contains(u.Id));
        }

        users = ApplySort(users, query.SortBy, query.RatingType, query.SortDescending);

        var totalCount = await users.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);

        var result = await users
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(u => u.ToUserResponse(query.JustOnline || onlineIds.Contains(u.Id)))
            .ToListAsync();




        var response = new UsersSearchResponse(result, totalPages);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options);

        return new UsersResult(response, false);
    }

    public async Task<ErrorOr<UserProfileResponse>> GetUserProfileAsync(string nickname)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        var isOnline = await _presenceService.IsOnlineAsync(user.Id);
        var response = user.ToUserProfileResponse(isOnline);
        return response;
    }
    public async Task<ErrorOr<FriendsProfileResponse>> GetFriendsProfileAsync(string nickname, PaginationQuery pagination)
    {

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        var allFriends = _dbContext.Friendships
            .Where(f => f.User.Nickname == nickname);


        var onlineIds = await _presenceService.GetOnlineIdsAsync(allFriends.Select(f => f.FriendId));

        var query = allFriends
        .OrderBy(f => f.Friend.Nickname)
            .Select(f => f.Friend.ToFriendProfileSummary(onlineIds.Contains(f.FriendId)));

        var totalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pagination.Limit);

        var friends = query
            .Skip((pagination.PageNumber - 1) * pagination.Limit)
            .Take(pagination.Limit);

        var response = new FriendsProfileResponse(
            await friends.ToListAsync(),
            totalPages


        );
        return response;
    }
    public async Task<ErrorOr<Success>> AddFriendAsync(string nickname, string currentUserNickname)
    {
        if (nickname == currentUserNickname)
        {
            return Error.Validation("sameUser", "You cannot add yourself as a friend.");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == currentUserNickname);
        var friend = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null || friend == null)
        {
            return Error.NotFound("userNotFound", "One or both users were not found.");
        }

        var existingFriendship = await _dbContext.Friendships
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

        _dbContext.Friendships.Add(friendship);
        _dbContext.Friendships.Add(reverseFriendship);
        await _dbContext.SaveChangesAsync();

        return Result.Success;

    }
    public async Task<ErrorOr<Success>> RemoveFriendAsync(string nickname, string currentUserNickname)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == currentUserNickname);
        var friend = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null || friend == null)
        {
            return Error.NotFound("userNotFound", "One or both users were not found.");
        }

        if (user.Id == friend.Id)
        {
            return Error.Validation("sameUser", "You cannot remove yourself from friends.");
        }
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == friend.Id);

        if (friendship == null)
        {
            return Error.Conflict("friendshipNotFound", "These users are not friends.");
        }

        var reverseFriendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.UserId == friend.Id && f.FriendId == user.Id);

        _dbContext.Friendships.Remove(friendship);
        if (reverseFriendship != null)
        {
            _dbContext.Friendships.Remove(reverseFriendship);
        }
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
    public async Task<ErrorOr<UpdateUserBioResponse>> UpdateUserBioAsync(string nickname, UpdateUserBioRequest request)
    {
        if (nickname is null)
        {
            return Error.Unauthorized("invalidAccessToken", "Access token is invalid.");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        user.ProfileBio = request.Bio;
        await _dbContext.SaveChangesAsync();
        return new UpdateUserBioResponse(user.ProfileBio);
    }
    public async Task<ErrorOr<UpdateUserProfilePictureResponse>> UpdateUserProfilePictureAsync(string nickname, UpdateUserProfilePictureRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        var avatarUrl = await _storageService.UploadAvatarAsync(request.ProfilePictureFile.OpenReadStream(), user.Id.ToString(), request.ProfilePictureFile.ContentType);
        if (avatarUrl.IsError)
        {
            return Error.Failure("uploadFailed", "Failed to upload avatar.");
        }
        user.ProfilePictureUrl = avatarUrl.Value;
        await _dbContext.SaveChangesAsync();
        return new UpdateUserProfilePictureResponse(user.ProfilePictureUrl);
    }
    public async Task<ErrorOr<OnlineFriendsResponse>> GetOnlineFriendsAsync(string nickname, PaginationQuery pagination)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null)
        {
            return Error.NotFound("userNotFound", "User with the given nickname was not found.");
        }
        var friends = await _dbContext.Friendships
            .Where(f => f.UserId == user.Id)
            .Select(f => f.Friend)
            .ToListAsync();
        var friendIds = friends.Select(f => f.Id).ToList();
        var onlineSet = await _presenceService.GetOnlineIdsAsync(friendIds);

        var onlineFriends = friends
         .OrderBy(f => f.Nickname)
            .Where(f => onlineSet.Contains(f.Id))
            .Select(f => f.ToFriendOnlineSummary())
            .ToList();
        var totalPages = (int)Math.Ceiling(onlineFriends.Count / (double)pagination.Limit);

        var paged = onlineFriends

            .Skip((pagination.PageNumber - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .ToList();
        return new OnlineFriendsResponse(Friends: paged, TotalPages: totalPages);

    }
}