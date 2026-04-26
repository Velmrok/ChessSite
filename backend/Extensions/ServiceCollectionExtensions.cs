using backend.Models;
using backend.Services;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;



namespace backend.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IHomeService, HomeService>();

        services.AddSingleton<IPresenceService, PresenceService>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
       
        

        return services;
    }
    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails()
;        
        return services;
    }
}