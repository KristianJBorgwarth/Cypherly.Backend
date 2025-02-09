using Cypherly.ChatServer.API.Hubs;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.ChangeEvent;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Handlers;

public class ChangeEventHandler(IHubContext<ChangeEventHub> hubContext) : IChangeEventNotifier
{
    public async Task NotifyAsync(string connectionId, ChangeEvent changeEvent, CancellationToken cancellationToken = default)
    {
       await hubContext.Clients.Client(connectionId).SendAsync("ChangeNotification", changeEvent, cancellationToken);
    }
}