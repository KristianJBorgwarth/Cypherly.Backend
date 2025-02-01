using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.SagaOrchestrator.Saga.User.Delete;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.SagaOrchestrator.Messaging.Test.Unit.Sagas.User;

public class UserDeleteSagaTest
{
    [Fact]
    public async Task UserDeleteSaga_Should_Transition_To_DeletingUserProfile_When_Received_UserDeleteMessage()
    {
        // Arrange
        var provider = new ServiceCollection().AddMassTransitTestHarness(cfg =>
        {
            cfg.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>()
                .InMemoryRepository();
        }).BuildServiceProvider();

        var harness = provider.GetTestHarness();
        await harness.Start();

        var sagaHarness = harness.GetSagaStateMachineHarness<UserDeleteSaga, UserDeleteSagaState>();
        var correlationId = Guid.NewGuid();


        // Act
        await harness.Bus.Publish(new UserDeletedMessage(Guid.NewGuid(), "test@mail.dk", correlationId));

        // Assert
        var result = await sagaHarness.Consumed.Any<UserDeletedMessage>();
        result.Should().BeTrue();
        var instance = sagaHarness.Created.Contains(correlationId);
        instance.CurrentState.Should().Be("DeletingUserProfile");
        await harness.Stop();
    }

    [Fact]
    public async Task UserDeleteSaga_Should_Transition_To_Failed_When_Received_UserProfileDeleteFault()
    {
        // Arrange
        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>()
                    .InMemoryRepository();

                // Add a consumer that fails when processing UserProfileDeleteMessage to simulate a fault
                cfg.AddConsumer<FaultyUserProfileDeleteConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        var sagaHarness = harness.GetSagaStateMachineHarness<UserDeleteSaga, UserDeleteSagaState>();
        var correlationId = Guid.NewGuid();

        // Act
        // 1. Publish UserDeletedMessage to create the saga instance and transition to DeletingUserProfile
        var userDeletedMessage = new UserDeletedMessage(correlationId, "test@mail.dk", correlationId);
        await harness.Bus.Publish(userDeletedMessage);

        // Assert that the saga instance is created and in the DeletingUserProfile state
        (await sagaHarness.Consumed.Any<UserDeletedMessage>()).Should().BeTrue();

        var instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("DeletingUserProfile");

        // 2. Publish UserProfileDeleteMessage, which will fail in the FaultyUserProfileDeleteConsumer
        var userProfileDeleteMessage = new UserProfileDeleteMessage(correlationId, correlationId, Guid.NewGuid());
        await harness.Bus.Publish(userProfileDeleteMessage);

        // Assert that the saga instance transitioned to Failed state due to the fault
        (await sagaHarness.Consumed.Any<Fault<UserProfileDeleteMessage>>()).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("Failed");
        await harness.Stop();
    }

    [Fact]
    public async Task UserDeleteSaga_Should_Transition_To_Finished_When_Received_OperationSuccededMessage()
    {
        // Arrange
        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>()
                    .InMemoryRepository();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        var sagaHarness = harness.GetSagaStateMachineHarness<UserDeleteSaga, UserDeleteSagaState>();
        var correlationId = Guid.NewGuid();

        // Act
        // 1. Publish UserDeletedMessage to create the saga instance and transition to DeletingUserProfile
        var userDeletedMessage = new UserDeletedMessage(correlationId, "test@mail.dk", correlationId);
        await harness.Bus.Publish(userDeletedMessage);

        // Assert that the saga instance is created and in the DeletingUserProfile state
        (await sagaHarness.Consumed.Any<UserDeletedMessage>()).Should().BeTrue();

        var instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("DeletingUserProfile");

        // 2. Publish OperationSuccededMessage to transition to SendingEmail state
        var operationSuccededMessage = new OperationSuccededMessage(OperationType.UserProfileDelete, correlationId);

        await harness.Bus.Publish(operationSuccededMessage);

        // Assert that the saga instance transitioned to Sending Email state due to the OperationSuccededMessage
        (await sagaHarness.Consumed.Any<OperationSuccededMessage>()).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("SendingEmail");
    }

    [Fact]
    public async Task UserDeleteSaga_Should_Transition_To_Failed_When_Sending_Email_Fails()
    {
        // Arrange
        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>()
                    .InMemoryRepository();

                // Add a consumer that fails when processing SendEmailMessage to simulate a fault
                cfg.AddConsumer<FaultySendEmailConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        var sagaHarness = harness.GetSagaStateMachineHarness<UserDeleteSaga, UserDeleteSagaState>();
        var correlationId = Guid.NewGuid();

        // Act
        // 1. Publish UserDeletedMessage to create the saga instance and transition to DeletingUserProfile
        var userDeletedMessage = new UserDeletedMessage(correlationId, "test@mail.dk", correlationId);
        await harness.Bus.Publish(userDeletedMessage);

        // Assert that the saga instance is created and in the DeletingUserProfile state
        (await sagaHarness.Consumed.Any<UserDeletedMessage>()).Should().BeTrue();

        var instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("DeletingUserProfile");

        // 2. Publish OperationSuccededMessage to transition to SendingEmail state
        var operationSuccededMessage = new OperationSuccededMessage(OperationType.UserProfileDelete, correlationId);
        await harness.Bus.Publish(operationSuccededMessage);

        // Assert that the saga instance transitioned to Sending Email state due to the OperationSuccededMessage
        (await sagaHarness.Consumed.Any<OperationSuccededMessage>()).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("SendingEmail");

        // 3. Publish SendEmailMessage, which will fail in the FaultySendEmailConsumer
        var sendEmailMessage = new SendEmailMessage("test@mail.dk",  "test", "test", correlationId);
        await harness.Bus.Publish(sendEmailMessage);

        // Assert that the saga instance transitioned to Failed state due to the fault
        (await sagaHarness.Consumed.Any<Fault<SendEmailMessage>>()).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("Failed");
        await harness.Stop();
    }

    [Fact]
    public async Task UserDeleteSaga_Should_Finish_When_All_Events_Are_Handled()
    {
        // Arrange
        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>()
                    .InMemoryRepository();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        var sagaHarness = harness.GetSagaStateMachineHarness<UserDeleteSaga, UserDeleteSagaState>();
        var correlationId = Guid.NewGuid();

        // Act
        // 1. Publish UserDeletedMessage to create the saga instance and transition to DeletingUserProfile
        var userDeletedMessage = new UserDeletedMessage(correlationId, "test@mail.dk", correlationId);
        await harness.Bus.Publish(userDeletedMessage);

        // Assert that the saga instance is created and in the DeletingUserProfile state
        (await sagaHarness.Consumed.Any<UserDeletedMessage>()).Should().BeTrue();

        var instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("DeletingUserProfile");

        // 2. Publish OperationSuccededMessage to transition to SendingEmail state
        var operationSuccededMessage1 = new OperationSuccededMessage(OperationType.UserProfileDelete, correlationId);
        await harness.Bus.Publish(operationSuccededMessage1);

        // Assert that the saga instance transitioned to SendingEmail state due to the OperationSuccededMessage
        (await sagaHarness.Consumed.Any<OperationSuccededMessage>()).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("SendingEmail");

        // 3. Publish OperationSuccededMessage for SendEmail to transition to Finished state
        var operationSuccededMessage2 = new OperationSuccededMessage(OperationType.SendEmail, correlationId);
        await harness.Bus.Publish(operationSuccededMessage2);

        // Assert that the saga instance transitioned to Finished state due to the OperationSuccededMessage
        (await sagaHarness.Consumed.Any<OperationSuccededMessage>(x => x.Context.Message.OperationType == OperationType.SendEmail)).Should().BeTrue();

        instance = sagaHarness.Created.Contains(correlationId);
        instance.Should().NotBeNull();
        instance.CurrentState.Should().Be("Final");
        await harness.Stop();
    }
}

public class FaultyUserProfileDeleteConsumer : IConsumer<UserProfileDeleteMessage>
{
    public Task Consume(ConsumeContext<UserProfileDeleteMessage> context)
    {
        throw new Exception("Simulated failure during UserProfileDelete");
    }
}

public class FaultySendEmailConsumer : IConsumer<SendEmailMessage>
{
    public Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        throw new Exception("Simulated failure during SendEmail");
    }
}