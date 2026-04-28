using backend.DTO.Common;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

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
    public async Task<string> Heartbeat()
    {
        string? userId = GetUserId();
        if (userId != null)
        {
            await _presenceService.SetOnlineAsync(Guid.Parse(userId));
            return "ok";
        }
        return "not_ok";
    }


    protected string? GetUserId() =>
        Context.User?.FindFirst("sub")?.Value;

    protected string? GetNickname() =>
        Context.User?.FindFirst("nickname")?.Value;
    

    public async Task<SignalRResponse> LeaveGroup(SignalRRequest request)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.Type);
        return new SignalRResponse(
            request.Type,
            request.CorrelationId,
            null
        );
    }
     public async Task<SignalRResponse> JoinGroup(SignalRRequest request)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, request.Type);
        return new SignalRResponse(
            request.Type,
            request.CorrelationId,
            null
        );
    }
}
    
