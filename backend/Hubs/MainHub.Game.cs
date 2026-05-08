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
    public async Task<SignalRResponse<EmptyResponse>> MakeMove(SignalRRequest<MakeMovePayload> request)
    {
        if (ValidatePayload<MakeMovePayload,EmptyResponse>(request) is { } error) return error;

        var userId = GetUserId();
        
        var result = await _gamesService.MakeMoveAsync(userId, request);
        return HandleError<EmptyResponse, EmptyResponse, MakeMovePayload>(result, request);
    } 
    [Authorize]
    public async Task<SignalRResponse<EmptyResponse>> SurrenderGame(SignalRRequest<SurrenderGamePayload> request)
    {
        if (ValidatePayload<SurrenderGamePayload,EmptyResponse>(request) is { } error) return error;

        var userId = GetUserId();
    
        
        var result = await _gamesService.SurrenderGameAsync(userId, request.Payload!.GameId);
        return HandleError<EmptyResponse, EmptyResponse, SurrenderGamePayload>(result, request); 
    } 
    [Authorize]
    public async Task<SignalRResponse<CreateAiGameResponse>> PlayWithAi(SignalRRequest<CreateAiGamePayload> request)
    {
        if (ValidatePayload<CreateAiGamePayload,CreateAiGameResponse>(request) is { } error) return error;

        var userId = GetUserId();
    
        
        var result = await _gamesService.CreateGameAsync(userId,_aiuserId,request.Payload!.Time,request.Payload.Increment,request.Payload.Difficulty);
        
        return HandleError(result, request,(x)=>new CreateAiGameResponse(x)); 
    }
}