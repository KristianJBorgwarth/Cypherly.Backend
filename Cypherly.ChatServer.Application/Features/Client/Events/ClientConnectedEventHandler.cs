using Cypherly.Application.Abstractions;
using Cypherly.ChatServer.Domain.Events.Client;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Client;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Events;

public class ClientConnectedEventHandler(
    IProducer<ClientConnectedMessage> producer,
    ILogger<ClientConnectedEventHandler> logger)
    : IDomainEventHandler<ClientConnectedEvent>
{
    public async Task Handle(ClientConnectedEvent notification, CancellationToken cancellationToken)
    {
        var message = new ClientConnectedMessage(notification.ClientId, Guid.NewGuid());

        await producer.PublishMessageAsync(message, cancellationToken);

        logger.LogInformation("Client connected: {ClientId}", notification.ClientId);
    }
}