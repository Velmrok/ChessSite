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
            var userId = GetUserId();
            if (userId != null)
                await _presenceService.SetOnlineAsync(Guid.Parse(userId));
            
            await base.OnConnectedAsync();
        }
        public async Task Heartbeat()
        {
            string? userId = GetUserId();
            if (userId != null)
                await _presenceService.SetOnlineAsync(Guid.Parse(userId));
        }

        protected string? GetUserId() =>
            Context.UserIdentifier;

        protected string? GetNickname() =>
            Context.User?.FindFirst("nickname")?.Value;
    }
}