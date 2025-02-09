using Cypherly.ChatServer.API.Hubs;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.ChangeEvent;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Handlers;

public class ChangeEventHandler(IHubContext<ChangeEventHub> context) : IChangeEventNotifier
{
    public Task NotifyAsync(string ConnectionId, ChangeEvent changeEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}