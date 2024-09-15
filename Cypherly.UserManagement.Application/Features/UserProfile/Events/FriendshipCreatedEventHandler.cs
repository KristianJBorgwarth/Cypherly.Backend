using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Domain.Events.UserProfile;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public class FriendshipCreatedEventHandler : IDomainEventHandler<FriendshipCreatedEvent>
{
    //TODO: Implement FriendshipCreatedEventHandler with some notification logic to notify the recipient of the friendship request
    public Task Handle(FriendshipCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.Write("Friendship created between {0} and {1}", notification.InitiatorId, notification.IntiateeId);
        return Task.CompletedTask;
    }
}