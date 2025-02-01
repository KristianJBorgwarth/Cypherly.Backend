using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public class MessageHub : Hub
{
    public async Task TestSendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}