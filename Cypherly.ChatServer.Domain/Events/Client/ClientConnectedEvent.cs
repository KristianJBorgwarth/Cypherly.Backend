using Cypherly.Domain.Events;

namespace Cypherly.ChatServer.Domain.Events.Client;

public sealed record ClientConnectedEvent(Guid DeviceId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}