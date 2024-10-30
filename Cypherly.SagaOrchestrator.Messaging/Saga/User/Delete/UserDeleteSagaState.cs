using Cypherly.SagaOrchestrator.Messaging.Abstractions;

namespace Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;

public sealed class UserDeleteSagaState : BaseState
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
}