using backend.Services;
using backend.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtGenerator,JwtGenerator>();
        return services;
    }
    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails()
;        
        return services;
    }
}