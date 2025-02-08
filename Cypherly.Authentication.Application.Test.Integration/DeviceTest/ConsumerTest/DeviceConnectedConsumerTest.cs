using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Consumers;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Common.Messaging.Messages.PublishMessages.Client;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.DeviceTest.ConsumerTest;

public class DeviceConnectedConsumerTest : IntegrationTestBase
{
    private readonly DeviceConnectedConsumer _sut;
    public DeviceConnectedConsumerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeviceConnectedConsumer>>();
        _sut = new DeviceConnectedConsumer(repo, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Context_Should_Update_Device_LastSeen()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test98KJl?????"), true);

        var device = new Device(Guid.NewGuid(), "test", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var message = new ClientConnectedMessage(device.Id, Guid.NewGuid());

        var fakeConsumeContext = A.Fake<ConsumeContext<ClientConnectedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.Device.AsNoTracking().First().LastSeen.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 45, 0));
    }

    [Fact]
    public async Task Handle_Given_Throws_Exception_Should_Not_Update_Device()
    {
        //Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test98KJl?????"), true);

        var device = new Device(Guid.NewGuid(), "test", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        device.SetDelete(); // Set device to deleted to throw exception

        user.AddDevice(device);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var message = new ClientConnectedMessage(device.Id, Guid.NewGuid());

        var fakeConsumeContext = A.Fake<ConsumeContext<ClientConnectedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        var act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        Db.Device.AsNoTracking().First().LastSeen.Should().BeNull();
    }
}