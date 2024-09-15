using Cypherly.Domain.Events;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public class UserProfilePictureUpdatedEvent(Guid UserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}