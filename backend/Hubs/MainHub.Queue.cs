using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Interfaces;
using backend.DTO.Common;
using backend.DTO.Queue;
using ErrorOr;

namespace backend.Hubs;

public partial class MainHub : Hub
{

    [Authorize]
    public async Task<SignalRResponse<EmptyResponse>> JoinQueue(SignalRRequest<JoinQueuePayload> request)
    {
        var payload = request.Payload;
        if (ValidatePayload<JoinQueuePayload,EmptyResponse>(request) is { } error) return error;


        var result = await _queueService.JoinQueueAsync(GetUserId(), payload!);
        return HandleError<EmptyResponse, EmptyResponse, JoinQueuePayload>(result, request);
    }
    [Authorize]
    public async Task<SignalRResponse<EmptyResponse>> LeaveQueue(SignalRRequest<EmptyPayload> request)
    {
        var result = await _queueService.LeaveQueueAsync(GetUserId());
        return HandleError<EmptyResponse, EmptyResponse, EmptyPayload>(result, request);
    }
}