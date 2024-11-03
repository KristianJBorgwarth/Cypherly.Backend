using Cypherly.Domain.Events;

namespace Cypherly.Authentication.Domain.Events.User;

public sealed record UserDeletedEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}