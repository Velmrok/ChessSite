using backend.Data;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.Hubs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;


namespace backend.Services.BackgroundServices;
public class HomeBackgroundService : BackgroundService
{
    private readonly IHubContext<MainHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HomeBackgroundService> _logger;


    public HomeBackgroundService(
        IHubContext<MainHub> hubContext,
        IServiceScopeFactory scopeFactory,
        ILogger<HomeBackgroundService> logger)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        ;
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
        var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();

        var usersOnline = await presenceService.GetOnlineCountAsync();
        var usersInQueue = await queueService.GetQueueLengthAsync();
        var matchesInProgress = -1; // TODO
        var createdAccounts = -1; // TODO

        await _hubContext.Clients
            .Group("Home")
            .SendAsync("StatsUpdated", new SignalRResponse<HomeInfoResponse>(
                Type: "StatsUpdated",
                CorrelationId: Guid.NewGuid().ToString(),
                Data: new HomeInfoResponse(
                    usersOnline,
                    usersInQueue,
                    matchesInProgress,
                    createdAccounts
                )
            ));

    }
}