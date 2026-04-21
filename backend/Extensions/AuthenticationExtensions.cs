using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backend.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                var jwt = configuration.GetSection("Jwt");
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["Key"]!)
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("accessToken", out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Title = "unauthorized",
                            Status = StatusCodes.Status401Unauthorized
                        });
                    }
                };

            });

            return services;
    }

    public static IServiceCollection AddCustomModelValidation(
        this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var firstErrorCode = context.ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "invalidRequest";

                return new BadRequestObjectResult(new ProblemDetails
                {
                    Title = firstErrorCode,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Validation failed"
                });
            };
        });

        return services;
    }
}