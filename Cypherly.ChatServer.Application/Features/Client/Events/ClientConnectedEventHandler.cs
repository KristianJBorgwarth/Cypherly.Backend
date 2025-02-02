using Cypherly.Application.Abstractions;
using Cypherly.ChatServer.Domain.Events.Client;

namespace Cypherly.ChatServer.Application.Features.Client.Events;

public class ClientConnectedEventHandler : IDomainEventHandler<ClientConnectedEvent>
{
    public async Task Handle(ClientConnectedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}