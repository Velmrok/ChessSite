
using System.Linq.Expressions;
using backend.Data;
using backend.DTO.Users;
using backend.Models;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    public UsersService(AppDbContext context)
    {
        _context = context;
    }

    private static readonly Dictionary<UsersSortBy, Expression<Func<User, object>>> SortMap = new()
    {



    };

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
        var users = _context.Users.AsQueryable();
        
       
        if (!string.IsNullOrEmpty(query.Search))
        {
            users = users.Where(u => u.Nickname.Contains(query.Search));
        }

        users = ApplySort(users, query.SortBy, query.RatingType, query.SortDescending);

        var result = await users
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(u => u.ToUserResponse())
            .ToListAsync();
        var totalCount = await users.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.Limit);
        return new UsersResponse(result, totalPages);
    }
}
