using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Consumers;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.ConsumerTest;

public class RollbackUserDeleteConsumerTest : IntegrationTestBase
{
    private readonly RollbackUserDeleteConsumer _sut;
    public RollbackUserDeleteConsumerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var service = scope.ServiceProvider.GetRequiredService<IUserLifeCycleService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RollbackUserDeleteConsumer>>();
        _sut = new RollbackUserDeleteConsumer(repo, service, unitOfWork, logger);
    }

    [Fact]
    public async Task Consume_Valid_Message_Should_Revert_SoftDelete()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test?239923KL"), true);
        user.SetDelete();
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var message = new UserDeleteFailedMessage(user.Id, Guid.NewGuid(), null, ServiceType.AuthenticationService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.User.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task Consume_Invalid_Message_Should_Not_Revert_SoftDelete()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test?239923KL"), true);
        user.SetDelete();
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        // Invalid ID
        var message =
            new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.AuthenticationService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        Func<Task> act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        Db.User.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().NotBeNull();
    }

}