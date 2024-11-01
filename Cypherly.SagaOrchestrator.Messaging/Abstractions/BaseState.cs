using MassTransit;

namespace Cypherly.SagaOrchestrator.Messaging.Abstractions;

public abstract class BaseState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public string? Error { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}