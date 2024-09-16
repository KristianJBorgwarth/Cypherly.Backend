using Cypherly.Domain.Events;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record UserProfileDisplayNameUpdatedEvent(Guid UserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}