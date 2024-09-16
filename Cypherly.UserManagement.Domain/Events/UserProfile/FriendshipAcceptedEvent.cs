using Cypherly.Domain.Events;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record FriendshipAcceptedEvent(Guid UserProfileId, string FriendTag) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}