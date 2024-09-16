using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Domain.Events.UserProfile;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public sealed class FriendshipAcceptedEventHandler : IDomainEventHandler<FriendshipAcceptedEvent>
{
    //TODO: Implement handling of notification here when chat server is implemented
    public async Task Handle(FriendshipAcceptedEvent notification, CancellationToken cancellationToken)
    {
        Console.Write("Friendship accepted between {0} and {1}", notification.UserProfileId, notification.FriendTag);
        await Task.CompletedTask;
    }
}