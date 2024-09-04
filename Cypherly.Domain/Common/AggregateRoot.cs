using Cypherly.Domain.Events;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Cypherly.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        Console.Write("Adding domain event: ");
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}