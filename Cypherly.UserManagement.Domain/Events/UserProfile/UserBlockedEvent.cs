using Cypherly.Domain.Events;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed  record UserBlockedEvent(Guid UserProfileId, Guid BlockedUserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}