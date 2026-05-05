using System.Collections.Concurrent;
using backend.Services.Interfaces;

namespace backend.Services
{
   
    public class GameTimerService : IGameTimerService, IDisposable
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _timers = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GameTimerService> _logger;

        public GameTimerService(IServiceScopeFactory scopeFactory, ILogger<GameTimerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void ScheduleTimeout(string gameId, string playerId, int remainingTimeMs)
        {
          
            CancelTimeout(gameId);

            var cts = new CancellationTokenSource();
            _timers[gameId] = cts;

            _ = RunTimerAsync(gameId, playerId, remainingTimeMs, cts.Token);
        }

        public void CancelTimeout(string gameId)
        {
            if (_timers.TryRemove(gameId, out var oldCts))
            {
                oldCts.Cancel();
                oldCts.Dispose();
            }
        }

        public void RemoveGame(string gameId)
        {
            CancelTimeout(gameId);
        }

        private async Task RunTimerAsync(string gameId, string playerId, int delayMs, CancellationToken ct)
        {
            try
            {
                if (delayMs <= 0)
                {
                    delayMs = 1;
                }

                await Task.Delay(delayMs, ct);

                _timers.TryRemove(gameId, out _);

                using var scope = _scopeFactory.CreateScope();
                var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();

                await gamesService.HandleTimeoutAsync(gameId, playerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Timer error for game {GameId}", gameId);
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _timers)
            {
                kvp.Value.Cancel();
                kvp.Value.Dispose();
            }
            _timers.Clear();
        }
    }
}