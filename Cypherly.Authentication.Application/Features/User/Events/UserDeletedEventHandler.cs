using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Messaging.PublishMessages;
using Cypherly.Application.Contracts.Messaging.PublishMessages.User;
using Cypherly.Authentication.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Events;

public class UserDeletedEventHandler(
    IProducer<UserDeletedMessage> producer,
    ILogger<UserDeletedEventHandler> logger)
    : IDomainEventHandler<UserDeletedEvent>
{
    public Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}