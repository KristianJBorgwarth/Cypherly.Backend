using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Consumers;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using MassTransit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.ConsumerTest;

public class RollbackUserProfileDeleteConsumerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly IUserProfileLifecycleService _fakeLifecycleService;
    private readonly RollbackUserProfileDeleteConsumer _sut;

    public RollbackUserProfileDeleteConsumerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakeLifecycleService = A.Fake<IUserProfileLifecycleService>();
        var fakeLogger = A.Fake<ILogger<RollbackUserProfileDeleteConsumer>>();
        _sut = new RollbackUserProfileDeleteConsumer(_fakeRepo, _fakeLifecycleService, _fakeUow, fakeLogger);
    }

    [Fact]
    public async Task Consume_WhenUserNotFound_Returns_And_No_Soft_Delete()
    {
        // Arrange
        var message = new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(),null, ServiceType.UserManagementService);
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserId)).Returns<UserProfile?>(null);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);


        // Assert
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeLifecycleService.RevertSoftDelete(A<UserProfile>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Consume_When_Valid_Request_Should_Revert_Soft_Delete()
    {
        // Arrange
        var message =
            new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.UserManagementService);
        var user = new UserProfile(Guid.NewGuid(), "james", UserTag.Create("james"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserId)).Returns(user);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeLifecycleService.RevertSoftDelete(user)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Consume_When_ServiceType_Not_UserManagementService_Should_DoNothing()
    {
        // Arrange
        var message =
            new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.AuthenticationService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeRepo.GetByIdAsync(A<Guid>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeLifecycleService.RevertSoftDelete(A<UserProfile>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }
}