using Cypherly.Domain.Events;

namespace Cypherly.Authentication.Domain.Events.User;

public sealed record DeviceCreatedEvent(Guid UserId, Guid DeviceId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}