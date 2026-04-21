using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("auth", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "tooManyRequests",
                    Status = StatusCodes.Status429TooManyRequests,
                    Detail = "You have exceeded the allowed number of requests. Please try again later."
                }, cancellationToken: token);
            };
        });

        return services;
    }
}