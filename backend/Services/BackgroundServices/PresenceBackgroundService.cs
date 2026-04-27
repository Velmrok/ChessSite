using backend.Data;
using backend.Hubs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace backend.Services.BackgroundServices
{
    public class PresenceBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;


        public PresenceBackgroundService(
            IServiceScopeFactory scopeFactory)
        {

            _scopeFactory = scopeFactory;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupOnlineStatuses();

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        private async Task CleanupOnlineStatuses()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();

            await presenceService.CleanUpAsync();
        }
    }
}