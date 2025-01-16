using Cypherly.SagaOrchestrator.Abstractions;

namespace Cypherly.SagaOrchestrator.Saga.User.Delete;

public sealed class UserDeleteSagaState : BaseState
{
    public Guid UserId { get; private set; }
    public string? Email { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    public void SetEmail(string email) => Email = email;
}