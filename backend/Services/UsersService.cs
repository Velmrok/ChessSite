
using System.Linq.Expressions;
using System.Text.Json;
using backend.Data;
using backend.DTO.Users;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    public UsersService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private static IQueryable<User> ApplySort(IQueryable<User> users, UsersSortBy sortBy, RatingType ratingType, bool descending)
    {
        if (sortBy == UsersSortBy.Rating)
            return ratingType switch
            {
                RatingType.Blitz => users.OrderByField(u => u.BlitzRating, descending),
                RatingType.Bullet => users.OrderByField(u => u.BulletRating, descending),
                _ => users.OrderByField(u => u.RapidRating, descending),
            };

        return sortBy switch
        {
            UsersSortBy.Nickname => users.OrderByField(u => u.Nickname, descending),
            UsersSortBy.LastActive => users.OrderByField(u => u.LastActive, descending),
            //UsersSortBy.OnlineStatus => users.OrderByField(u => u.OnlineStatus, descending),
            _ => users.OrderByField(u => u.CreatedAt, descending),
        };
    }
    public async Task<UsersResponse> GetAllUsersAsync(GetUsersQuery query)
    {
         var cacheKey = $"users:{query.Page}:{query.Limit}:{query.Search}:" +
                       $"{query.SortBy}:{query.SortDescending}:{query.RatingType}:" +
                       $"{query.MinRating}:{query.MaxRating}:{query.Online}";

        
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<UsersResponse>(cached)!;
        }

        var users = _context.Users.AsQueryable();
        
       
        if (!string.IsNullOrEmpty(query.Search))
        {
            users = users.Where(u => u.Nickname.ToLower().Contains(query.Search.ToLower()));
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

        return response;
    }
}
