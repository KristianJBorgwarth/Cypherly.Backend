using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Consumers;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.ConsumerTest;

public class RollbackUserDeleteConsumerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly IUserLifeCycleServices _fakeLifeCycleServices;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly ILogger<RollbackUserDeleteConsumer> _fakeLogger;
    private readonly RollbackUserDeleteConsumer _sut;

    public RollbackUserDeleteConsumerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeLifeCycleServices = A.Fake<IUserLifeCycleServices>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeLogger = A.Fake<ILogger<RollbackUserDeleteConsumer>>();
        _sut = new RollbackUserDeleteConsumer(_fakeRepo, _fakeLifeCycleServices, _fakeUnitOfWork, _fakeLogger);
    }

    [Fact]
    public async Task Consume_Valid_Message_Should_Revert_Soft_Delete()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test123KLJJSD?"), false);
        user.SetDelete();

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        var message = new UserDeleteFailedMessage(user.Id, Guid.NewGuid(), null, ServiceType.AuthenticationService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeLifeCycleServices.RevertSoftDelete(user)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_Message_With_Invalid_ServiceType_Should_Do_Nothing()
    {
        // Arrange
        var message =
            new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.UserManagementService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeRepo.GetByIdAsync(A<Guid>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeLifeCycleServices.RevertSoftDelete(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Consume_When_User_Does_Not_Exist_Should_Do_Nothing()
    {
        var message = new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.AuthenticationService);

        A.CallTo(()=> _fakeRepo.GetByIdAsync(A<Guid>._)).Returns<User?>(null);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        Func<Task> act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}