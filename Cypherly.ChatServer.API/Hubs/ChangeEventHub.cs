using Cypherly.ChatServer.Application.Features.ChangeEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

[Authorize]
public class ChangeEventHub : Hub
{
    public async Task SendChangeNotification(string connectionId, ChangeEvent changeEvent)
    {
        await Clients.Client(connectionId).SendAsync("ChangeNotification", changeEvent);
    }
}