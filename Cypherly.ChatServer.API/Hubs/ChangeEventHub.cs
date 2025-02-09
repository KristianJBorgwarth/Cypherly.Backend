using Cypherly.ChatServer.Application.Features.ChangeEvent;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public class ChangeEventHub : Hub
{
    public async Task SendChangeNotification(string connectionId, ChangeEvent changeEvent)
    {
        await Clients.Client(connectionId).SendAsync("ChangeNotification", changeEvent);
    }
}