using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Interfaces;
using backend.DTO.Games;
using backend.DTO.Common;

namespace backend.Hubs;

public partial class MainHub : Hub
{


    [Authorize]
    public async Task<SignalRResponse<MoveInfo>> MakeMove(SignalRRequest<MakeMoveRequest> request)
    {
        if (ValidatePayload<MakeMoveRequest,MoveInfo>(request) is { } error) return error;

        var userId = GetUserId();
        
        var result = await _gamesService.MakeMoveAsync(userId, request.Payload!);
        return HandleError(result, request, data =>
        {
            Clients.Group($"Game:{request.Payload!.GameId}")
                .SendAsync("MoveMade", new SignalRResponse<MoveInfo>(
                    Type: "MoveMade",
                    CorrelationId: request.Payload.GameId,
                    Data: data
                ));
            return data;
        });
    } 
}