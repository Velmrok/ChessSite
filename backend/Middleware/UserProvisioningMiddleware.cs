using backend.Data;
using backend.DTO.Common;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Middleware;

public class UserProvisioningMiddleware
{
    private readonly RequestDelegate _next;

    public UserProvisioningMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirst("sub")?.Value;
            if (sub != null && Guid.TryParse(sub, out var userId))
            {
                var exists = await dbContext.Users.AnyAsync(u => u.Id == userId);
                if (!exists)
                {
        
                    var nickname = context.User.FindFirst("preferred_username")?.Value
                                ?? "Player";

                    dbContext.Users.Add(new User
                    {
                        Id = userId,
                        Nickname = nickname,
          
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        await _next(context);
    }
}