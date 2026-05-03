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

        private record ScheduledTask(Func<Task> Action, TimeSpan Interval)
        {
            public DateTime LastRun { get; set; } = DateTime.UtcNow;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<ScheduledTask>
            {
                new(CleanupOnlineStatuses,       TimeSpan.FromSeconds(30)),
                new(FlushLastActiveStatuses,     TimeSpan.FromMinutes(2)),
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                foreach (var task in tasks)
                {
                    if (now - task.LastRun >= task.Interval)
                    {
                        await task.Action();
                        task.LastRun = now;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        private async Task CleanupOnlineStatuses()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();

            await presenceService.CleanUpAsync();
        }
        private async Task FlushLastActiveStatuses()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var presenceService = scope.ServiceProvider.GetRequiredService<IPresenceService>();

            await presenceService.FlushLastActiveAsync();
        }
    }
}