using backend.Data;
using backend.Hubs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

public class HomeBackgroundService : BackgroundService
{
    private readonly IHubContext<MainHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedCache _cache;
    private readonly ILogger<HomeBackgroundService> _logger;

    public HomeBackgroundService(
        IHubContext<MainHub> hubContext,
        IServiceScopeFactory scopeFactory, 
        IDistributedCache cache,
        ILogger<HomeBackgroundService> logger)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeBackgroundService error");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task SendStats()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();

        var usersOnline = await presenceService.GetOnlineCountAsync();
        var activeGames = -1; // TODO
        var totalUsers = -1; // TODO

        await _hubContext.Clients
            .Group("Home")
            .SendAsync("StatsUpdated", new
            {
                usersOnline,
                matchesInProgress = activeGames,
                createdAccounts = totalUsers
            });
    }
}