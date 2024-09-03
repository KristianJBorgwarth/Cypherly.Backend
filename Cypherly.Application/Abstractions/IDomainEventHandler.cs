using Cypherly.Domain.Events;
using MediatR;

namespace Cypherly.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{

}