using MediatR;

namespace Cypherly.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}