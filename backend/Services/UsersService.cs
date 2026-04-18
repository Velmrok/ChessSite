
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
        [UsersSortBy.RatingRapid] = u => u.RapidRating,
        [UsersSortBy.RatingBlitz] = u => u.BlitzRating,
        [UsersSortBy.RatingBullet] = u => u.BulletRating,
        [UsersSortBy.LastActive] = u => u.LastActive,
       // [UsersSortBy.OnlineStatus] = (u => u.IsOnline, true), TODO: add online status via redis
        [UsersSortBy.Nickname] = u => u.Nickname,
        [UsersSortBy.CreatedAt] = u => u.CreatedAt,
    };
     private static IQueryable<User> ApplySort(IQueryable<User> users, UsersSortBy sortBy, bool descending = true)
    {
        var keySelector = SortMap[sortBy];
        return descending ? users.OrderByDescending(keySelector) : users.OrderBy(keySelector);
    }
    public async Task<List<UserResponse>> GetAllUsersAsync(GetUsersQuery query)
    {
        var users = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(query.Search))
        {
            users = users.Where(u => u.Nickname.Contains(query.Search));
        }

        users = ApplySort(users, query.SortBy, query.SortDescending);

        var result = users
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(u => u.ToUserResponse())
            .ToListAsync();
        return await result;
    }
}
