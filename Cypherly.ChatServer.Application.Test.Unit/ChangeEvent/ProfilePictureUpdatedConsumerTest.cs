using Cypherly.ChatServer.Application.Cache.Client;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.ChangeEvent.ProfilePictureUpdated;
using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.Common.Messaging.Messages.PublishMessages.UserProfile;
using FakeItEasy;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Test.Unit.ChangeEvent;

public class ProfilePictureUpdatedConsumerTest
{
    private readonly IChangeEventNotifier _fakeChangeEventNotifier;
    private readonly IClientCache _fakeClientCache;
    private readonly ILogger<ProfilePictureUpdatedConsumer> _fakeLogger;

    private readonly ProfilePictureUpdatedConsumer _sut;

    public ProfilePictureUpdatedConsumerTest()
    {
        _fakeChangeEventNotifier = A.Fake<IChangeEventNotifier>();
        _fakeClientCache = A.Fake<IClientCache>();
        _fakeLogger = A.Fake<ILogger<ProfilePictureUpdatedConsumer>>();

        _sut = new ProfilePictureUpdatedConsumer(_fakeChangeEventNotifier, _fakeClientCache, _fakeLogger);
    }

    [Fact]
    public async Task Consume_Given_Valid_ConnectedClient_Should_NotifyClient()
    {
        // Arrange
        var client = new Client(Guid.NewGuid(), Guid.NewGuid());
        var profilePictureUpdatedMessage = new ProfilePictureUpdatedMessage(Guid.NewGuid(), "profilePictureUrl", new List<Guid> {  client.ConnectionId}, Guid.NewGuid());

        var clientCacheDto = ClientCacheDto.Create(client, "transientId");
        A.CallTo(() => _fakeClientCache.GetAsync(client.ConnectionId, CancellationToken.None)).Returns(clientCacheDto);

        var fakeConsumeContext = A.Fake<ConsumeContext<ProfilePictureUpdatedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(profilePictureUpdatedMessage);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeChangeEventNotifier.NotifyAsync(A<string>.That.IsEqualTo(client.ConnectionId.ToString()),
                A<Features.ChangeEvent.ChangeEvent>._,
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_Given_No_Clients_Should_Not_Notify()
    {
        var profilePictureUpdatedMessage = new ProfilePictureUpdatedMessage(Guid.NewGuid(), "profilePictureUrl", new List<Guid>(), Guid.NewGuid());
        var fakeConsumeContext = A.Fake<ConsumeContext<ProfilePictureUpdatedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(profilePictureUpdatedMessage);

        await _sut.Consume(fakeConsumeContext);

        A.CallTo(() => _fakeChangeEventNotifier.NotifyAsync(A<string>._, A<Features.ChangeEvent.ChangeEvent>._, A<CancellationToken>._))
            .MustNotHaveHappened();

    }
}