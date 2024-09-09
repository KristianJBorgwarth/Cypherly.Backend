using Cypherly.Application.Contracts.Messaging.PublishMessages;
using FakeItEasy;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.MassTransit.Messaging.Test;

public class ProducerTests
{
    private readonly IPublishEndpoint _fakePublishEndpoint;
    private readonly Producer<BaseMessage> _sut;

    public ProducerTests()
    {
        _fakePublishEndpoint = A.Fake<IPublishEndpoint>();
        var fakeLogger = A.Fake<ILogger<Producer<BaseMessage>>>();
        _sut = new(_fakePublishEndpoint, fakeLogger);
    }

    [Fact]
    public async Task PublishMessageAsync_WhenCalled_ShouldPublishMessage()
    {
        // Arrange
        var message = A.Fake<BaseMessage>();


        // Act
        await _sut.PublishMessageAsync(message, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakePublishEndpoint.Publish(message,  CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task PublishMessageAsync_WhenExceptionOccurs_ShouldLogErrorAndThrow()
    {
        // Arrange
        var message = A.Fake<BaseMessage>();

        var exception = new Exception("Some error");

        A.CallTo(() => _fakePublishEndpoint.Publish(message, CancellationToken.None))
            .Throws(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _sut.PublishMessageAsync(message, CancellationToken.None));

        A.CallTo(() => _fakePublishEndpoint.Publish(message, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }
}
