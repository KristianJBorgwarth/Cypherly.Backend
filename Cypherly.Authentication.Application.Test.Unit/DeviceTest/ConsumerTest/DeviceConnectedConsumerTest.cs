using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Consumers;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Client;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.DeviceTest.ConsumerTest;

public class DeviceConnectedConsumerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly DeviceConnectedConsumer _sut;

    public DeviceConnectedConsumerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var logger = A.Fake<ILogger<DeviceConnectedConsumer>>();
        _sut = new DeviceConnectedConsumer(_fakeRepo, _fakeUow, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Message_Should_Not_Fail()
    {
        // Arrange
        var user = new User();
        user.AddDevice(new Device());
        var message = new ClientConnectedMessage(user.Devices.First().Id, Guid.NewGuid());

        A.CallTo(() => _fakeRepo.GetByDeviceIdAsync(message.DeviceId)).Returns(user);

        var fakeConsumeContext = A.Fake<ConsumeContext<ClientConnectedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        user.Devices.First().LastSeen.Should().NotBeNull();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_User_Is_Null_Should_Throw_Exception()
    {
        // Arrange
        var message = new ClientConnectedMessage(Guid.NewGuid(), Guid.NewGuid());

        A.CallTo(() => _fakeRepo.GetByDeviceIdAsync(message.DeviceId)).Returns<User?>(null);

        var fakeConsumeContext = A.Fake<ConsumeContext<ClientConnectedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        Func<Task> act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

    }
}