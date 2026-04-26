using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public partial class MainHub : Hub
    {
        private readonly IGamesService _gameService;
        private readonly IHomeService _homeService;

        public MainHub(
            IGamesService gameService,
            IHomeService homeService
            )
        {
            _gameService = gameService;
            _homeService = homeService;

        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        protected string GetUserId() =>
            Context.UserIdentifier
            ?? throw new HubException("unauthorized");

        protected string GetNickname() =>
            Context.User?.FindFirst("nickname")?.Value
            ?? throw new HubException("unauthorized");
    }
}