using backend.Data;
using backend.DTO.Common;
using backend.DTO.Home;
using backend.DTO.Queue;
using backend.Enums;
using backend.Hubs;
using backend.Services.Interfaces;
using backend.Services.Mappers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;


namespace backend.Services.BackgroundServices;
public class QueueBackgroundService : BackgroundService
{
    
    private readonly IHubContext<MainHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QueueBackgroundService> _logger;
    private readonly IDatabase _db;
    private const string QueueSetKey = "chess:queue";
    private const int RatingThreshold = 200; 
    public QueueBackgroundService(
        IHubContext<MainHub> hubContext,
        IServiceScopeFactory scopeFactory,
        ILogger<QueueBackgroundService> logger,
        IConnectionMultiplexer redis)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _db = redis.GetDatabase();

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MatchLoop();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QueueBackgroundService error");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task MatchLoop()
    {
        foreach (QueueKey key in Enum.GetValues(typeof(QueueKey)))
        {
            var users = await GetUsersWithRatingsByQueueKeyAsync(key);
            if (users.Count < 2)
                continue;

            users.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // Sort by rating
            for (int i = 0; i < users.Count - 1; i += 2)
            {
                var user1 = users[i];
                var user2 = users[i + 1];
                
                var isMatched = await TryMatchPlayersAsync(user1, user2, key);
                if (!isMatched)
                    i--; // no pair for user1, try user2 with next user
                else
                {
                    await removeUserFromQueueAsync(user1.Item1);
                    await removeUserFromQueueAsync(user2.Item1);
                }
            }
        }
    }
    private async Task<bool> TryMatchPlayersAsync((string, int) user1, (string, int) user2, QueueKey key)
    {
        if (Math.Abs(user1.Item2 - user2.Item2) > RatingThreshold)
            return false;

        var gameService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IGamesService>();
        var createGameResult = await gameService.CreateGameAsync(user1.Item1, user2.Item1, key.GetTime(), key.GetIncrement());

        if (createGameResult.IsError)
        {
            _logger.LogError("Failed to create game for users {User1} and {User2}: {Error}", user1.Item1, user2.Item1, createGameResult.Errors[0].Description);
            return false;
        }
        var response = new SignalRResponse<MatchFoundResponse>(
            Type: "GameFound",
            CorrelationId: Guid.NewGuid().ToString(),
            Data: new MatchFoundResponse(GameUrl: $"/games/{createGameResult.Value}")
        );
        await _hubContext.Clients.User(user1.Item1).SendAsync("GameFound", response);
        await _hubContext.Clients.User(user2.Item1).SendAsync("GameFound", response);

        return true;
    }
    private async Task<List<(string,int)>> GetUsersWithRatingsByQueueKeyAsync(QueueKey key)
    {
        var ids = await GetUserIdsByQueueKeyAsync(key);
        List<(string, int)> usersWithRatings = [];
        foreach (var userId in ids)
        {
            var rating = await _db.HashGetAsync($"{QueueSetKey}:{userId}", "rating");
            usersWithRatings.Add((userId, int.Parse(rating.ToString())));
        }
        return usersWithRatings;
    }
    private async Task<List<string>> GetUserIdsByQueueKeyAsync(QueueKey key)
    {
        var redisKey = key.ToRedisKey();
        return await _db.SetMembersAsync(redisKey)
            .ContinueWith(t => t.Result.Select(v => v.ToString()).ToList());
    }
    private async Task removeUserFromQueueAsync(string userId)
    {
        var queueService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IQueueService>();
        await queueService.LeaveQueueAsync(userId);
    }
}
    