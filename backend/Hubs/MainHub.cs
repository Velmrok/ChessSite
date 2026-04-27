using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public partial class MainHub : Hub
    {
        private readonly IGamesService _gameService;
        private readonly IHomeService _homeService;
        private readonly IPresenceService _presenceService;
        public MainHub(
            IGamesService gameService,
            IHomeService homeService,
            IPresenceService presenceService
            )
        {
            _gameService = gameService;
            _homeService = homeService;
            _presenceService = presenceService;

        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = GetUserId();
                await _presenceService.SetOnlineAsync(Guid.Parse(userId));
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = GetUserId();
                await _presenceService.SetOfflineAsync(Guid.Parse(userId));
            }
            await base.OnDisconnectedAsync(exception);
        }
        protected string GetUserId() =>
            Context.UserIdentifier
            ?? throw new HubException("unauthorized");

        protected string GetNickname() =>
            Context.User?.FindFirst("nickname")?.Value
            ?? throw new HubException("unauthorized");
    }
}