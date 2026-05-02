using backend.DTO.Common;
using backend.DTO.Queue;
using backend.Services.Interfaces;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public partial class MainHub : Hub
{
    private readonly IGamesService _gameService;
    private readonly IHomeService _homeService;
    private readonly IPresenceService _presenceService;
        private readonly IQueueService _queueService;
    public MainHub(
        IGamesService gameService,
        IHomeService homeService,
        IQueueService queueService,
        IPresenceService presenceService
        )
    {
        _gameService = gameService;
        _homeService = homeService;
        _presenceService = presenceService;
        _queueService = queueService;

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
    

    public async Task<SignalRResponse<EmptyResponse>> LeaveGroup(SignalRRequest request)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.Type);
        return new SignalRResponse<EmptyResponse>(
            request.Type,
            request.CorrelationId,
            default
        );
    }
     public async Task<SignalRResponse<EmptyResponse>> JoinGroup(SignalRRequest request)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, request.Type);
        return new SignalRResponse<EmptyResponse>(
            request.Type,
            request.CorrelationId,
            default
        );
    }
    protected SignalRResponse<TResponse> HandleError<T, TResponse, TPayload>(
    ErrorOr<T> result,
    SignalRRequest<TPayload> request,
    Func<T, TResponse>? onSuccess = null)
    {
        if (result.IsError)
        {
            var error = result.FirstError;

            return new SignalRResponse<TResponse>(
                request.Type,
                request.CorrelationId,
                default,
                new SignalRError(
                    Title: error.Code,
                    Message: error.Description
                )
            );
        }

        var data = onSuccess != null
            ? onSuccess(result.Value)
            : default;

        return new SignalRResponse<TResponse>(
            request.Type,
            request.CorrelationId,
            data,
            null
        );
    }

}

