using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.Client.Consumers;
using Cypherly.ChatServer.Application.Test.Integration.Setup;
using Cypherly.ChatServer.Persistence.Context;
using Cypherly.Common.Messaging.Messages.RequestMessages.Client;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Cypherly.ChatServer.Application.Test.Integration.ConsumerTest;

public class CreateClientConsumerTest : IntegrationTestbase
{
    private readonly CreateClientConsumer _sut;
    public CreateClientConsumerTest(IntegrationTestFactory<Program, ChatServerDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateClientConsumer>>();

        _sut = new CreateClientConsumer(clientRepository, unitOfWork, logger);
    }


    [Fact]
    public async void Consume_ValidRequest_Should_Create_Client()
    {
        // Arrange
        var request = new CreateClientRequest(Guid.NewGuid(), Guid.NewGuid());

        var fakeConsumeContext = A.Fake<ConsumeContext<CreateClientRequest>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(request);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.Client.Count().Should().Be(1);
        Db.Client.First().Id.Should().Be(request.DeviceId);
        Db.Client.First().ConnectionId.Should().Be(request.ConnectionId);
    }
}