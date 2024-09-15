using Cypherly.Domain.Events;

namespace Cypherly.Authentication.Domain.Events.User;

public sealed record UserVerifiedEvent(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}