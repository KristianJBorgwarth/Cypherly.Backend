using MassTransit;

namespace Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;

internal sealed class UserDeleteState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
}