using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using MassTransit;
using Microsoft.Extensions.Logging;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;

public sealed class UserDeleteSaga : MassTransitStateMachine<UserDeleteSagaState>
{
    public Event<UserDeletedMessage> UserDeleteMessageReceived { get; private set; }
    public Event<OperationSuccededMessage> OperationSuccededReceived { get; private set; }
    public Event<Fault<UserProfileDeleteMessage>> UserProfileDeleteFault { get; private set; }
    public Event<Fault<SendEmailMessage>> SendEmailFault { get; private set; }
    public State DeletingUserProfile { get; private set; }
    public State SendingEmail { get; private set; }
    public State Failed { get; private set; }
    public State Finished { get; private set; }

    public UserDeleteSaga(ILogger<UserDeleteSaga> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => UserDeleteMessageReceived, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => UserProfileDeleteFault, x => x.CorrelateById(m => m.Message.Message.CorrelationId));
        Event(() => SendEmailFault, x => x.CorrelateById(m => m.Message.Message.CorrelationId));
        Event(() => OperationSuccededReceived, x => x.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(UserDeleteMessageReceived)
                .ThenAsync(async context =>
                {
                    context.Saga.SetUserId(context.Message.UserId);
                    context.Saga.SetEmail(context.Message.Email);
                    logger.LogInformation("Received UserDeletedMessage, publishing UserProfileDeleteMessage.");

                    await context.Publish(new UserProfileDeleteMessage(
                        context.Message.UserId,
                        context.Message.CorrelationId,
                        context.Message.Id));
                })
                .TransitionTo(DeletingUserProfile)
        );

        During(DeletingUserProfile,
            When(UserProfileDeleteFault)
                .ThenAsync(async context =>
                {
                    logger.LogError("UserProfileDelete faulted, rolling back, and failing saga with ID: {ID}.",
                        context.Saga.CorrelationId);

                    context.Saga.SetError(context.Message.Exceptions);

                    await context.Publish(new UserDeleteFailedMessage(
                        context.Saga.UserId,
                        context.Saga.CorrelationId,
                        context.Message.Message.Id,
                        ServiceType.AuthenticationService));
                })
                .TransitionTo(Failed)
                .Finalize(),
            When(OperationSuccededReceived)
                .If(context => context.Message.OperationType == OperationType.UserProfileDelete, binder =>
                    binder.ThenAsync(
                        async context =>
                        {
                            logger.LogInformation("UserProfileDelete succeded, publishing SendEmailMessage.");
                            await context.Publish(new SendEmailMessage(
                                context.Saga.Email!,
                                "User Deleted",
                                "Your account has been deleted.",
                                context.Saga.CorrelationId,
                                context.Message.Id));
                        }))
                .TransitionTo(SendingEmail));

        During(SendingEmail,
            When(SendEmailFault)
                .ThenAsync(async context =>
                {
                    logger.LogError("SendEmail faulted, rolling back, and failing saga with ID: {ID}.",
                        context.Saga.CorrelationId);

                    context.Saga.SetError(context.Message.Exceptions);

                    await context.Publish(new UserDeleteFailedMessage(
                        context.Saga.UserId,
                        context.Saga.CorrelationId,
                        context.Message.Message.Id,
                        ServiceType.AuthenticationService,
                        ServiceType.UserManagementService));
                })
                .TransitionTo(Failed),
            When(OperationSuccededReceived)
                .If(context => context.Message.OperationType == OperationType.SendEmail, binder =>
                    binder.Then(context =>
                    {
                        logger.LogInformation("SendEmail succeded, finalizing saga with ID: {ID}.",
                            context.Saga.CorrelationId);
                    }))
                .TransitionTo(Finished)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
