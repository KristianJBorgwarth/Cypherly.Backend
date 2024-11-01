using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Events;

public class UserDeletedEventHandler(
    IProducer<UserDeletedMessage> producer,
    ILogger<UserDeletedEventHandler> logger)
    : IDomainEventHandler<UserDeletedEvent>
{
    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User with id {UserId} and email {Email} has been deleted", notification.UserId, notification.Email);
        var message = new UserDeletedMessage(notification.UserId, notification.Email, notification.UserId);
        await producer.PublishMessageAsync(message, cancellationToken);
    }
}