using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Interfaces;
using backend.DTO.Games;
using backend.DTO.Common;
using ErrorOr;

namespace backend.Hubs;

public partial class MainHub : Hub
{


    [Authorize]
    public async Task<SignalRResponse<EmptyResponse>> MakeMove(SignalRRequest<MakeMoveRequest> request)
    {
        if (ValidatePayload<MakeMoveRequest,EmptyResponse>(request) is { } error) return error;

        var userId = GetUserId();
        
        var result = await _gamesService.MakeMoveAsync(userId, request);
        return HandleError(result, request);
    } 
    [Authorize]
    public async Task<SignalRResponse<EmptyResponse>> SurrenderGame(SignalRRequest<SurrenderGameRequest> request)
    {
        if (ValidatePayload<SurrenderGameRequest,EmptyResponse>(request) is { } error) return error;

        var userId = GetUserId();
    
        
        var result = await _gamesService.SurrenderGameAsync(userId, request.Payload!.GameId);
        return HandleError(result, request); 
    } 
}