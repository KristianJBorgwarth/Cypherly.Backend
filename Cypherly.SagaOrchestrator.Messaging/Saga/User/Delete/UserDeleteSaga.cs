using Cypherly.Application.Contracts.Messaging.PublishMessages.User;
using MassTransit;

namespace Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;

public sealed class UserDeleteSaga : MassTransitStateMachine<UserDeleteSagaState>
{
    public Event<UserDeletedMessage> UserDeleteMessageRecieved { get; private set; }

    public UserDeleteSaga()
    {
        InstanceState(x=> x.CurrentState);

        Event(() => UserDeleteMessageRecieved, x => x.CorrelateById(m => m.Message.CorrelationId));
    }

}