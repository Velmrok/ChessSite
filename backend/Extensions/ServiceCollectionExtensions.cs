using backend.Models;
using backend.Services;
using backend.Services.BackgroundServices;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;



namespace backend.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
  {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IQueueService, QueueService>();

        services.AddScoped<IGameRepository, GameRepository>();

        services.AddScoped<IPresenceService, PresenceService>();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        services.AddSingleton<IGameTimerService, GameTimerService>();


        services.AddHostedService<HomeBackgroundService>();
        services.AddHostedService<PresenceBackgroundService>();
        services.AddHostedService<QueueBackgroundService>();

        var engineUrl = Configuration.GetValue<string>("MyChessEngine:Url");
        services.AddHttpClient<IChessEngine, MyLocalChessEngine>(client =>
        {
            client.BaseAddress = new Uri(engineUrl ?? throw new InvalidOperationException("Chess engine URL is not configured."));
        });

        return services;
    }
    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails()
;
        return services;
    }
}