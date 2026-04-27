using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Interfaces;

namespace backend.Hubs;

public partial class MainHub : Hub
{
   
    public async Task<string> JoinHomeGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Home");
        await Clients.Caller.SendAsync("LobbyJoined");
        return "Joined home group";
    }
    public async Task<string> LeaveHomeGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Home");
        await Clients.Caller.SendAsync("LobbyLeft");
        return "Left home group";
    }
}