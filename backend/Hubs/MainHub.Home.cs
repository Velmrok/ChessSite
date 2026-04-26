using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Interfaces;

namespace backend.Hubs;
[Authorize]
public partial class MainHub : Hub
{
   
    public async Task<string> JoinHomeGroup()
    {
        var userNickname = GetNickname();
        await Groups.AddToGroupAsync(Context.ConnectionId, "Home");
        await Clients.Caller.SendAsync("LobbyJoined");
        return "ok";
    }
}