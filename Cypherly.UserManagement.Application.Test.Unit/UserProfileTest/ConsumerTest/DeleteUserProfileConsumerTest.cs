using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Consumers;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.ConsumerTest;

public class DeleteUserProfileConsumerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly IProducer<OperationSuccededMessage> _fakeProducer;
    private readonly IUserProfileService _fakeService;
    private readonly DeleteUserProfileConsumer _sut;

    public DeleteUserProfileConsumerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakeProducer = A.Fake<IProducer<OperationSuccededMessage>>();
        _fakeService = A.Fake<IUserProfileService>();
        _sut = new DeleteUserProfileConsumer(_fakeRepo, _fakeService, _fakeUow, _fakeProducer);
    }

    [Fact]
    public async Task Consume_When_User_Exists_Should_SoftDelete()
    {
        // Arrange
        var message = new UserProfileDeleteMessage(Guid.NewGuid(), Guid.NewGuid(), null);
        var user = new UserProfile();
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserProfileId)).Returns(user);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserProfileDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeService.SoftDelete(user)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeProducer.PublishMessageAsync(A<OperationSuccededMessage>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_When_User_Does_Not_Exist_Should_Throw_KeyNotFoundException()
    {
        // Arrange
        var message = new UserProfileDeleteMessage(Guid.NewGuid(), Guid.NewGuid(), null);
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserProfileId)).Returns<UserProfile?>(null);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserProfileDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        Func<Task> act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }


}