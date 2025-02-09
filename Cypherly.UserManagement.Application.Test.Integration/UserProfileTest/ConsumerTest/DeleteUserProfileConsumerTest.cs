using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Consumers;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.ConsumerTest;

public class DeleteUserProfileConsumerTest : IntegrationTestBase
{
    private readonly DeleteUserProfileConsumer _sut;
    public DeleteUserProfileConsumerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileLifecycleService>();
        var producer = scope.ServiceProvider.GetRequiredService<IProducer<OperationSuccededMessage>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeleteUserProfileConsumer>>();
        _sut = new DeleteUserProfileConsumer(userProfileRepository, userProfileService, unitOfWork, producer, logger);
    }

    [Fact]
    public async void Consume_Valid_Message_Should_SoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var message = new UserDeleteMessage(userProfile.Id, Guid.NewGuid());

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async void Consume_Invalid_Message_Should_Throw_KeyNotFoundException()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();
        var invalidId = Guid.NewGuid();

        var message = new UserDeleteMessage(invalidId, Guid.NewGuid());

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        Func<Task> act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().BeNull();
    }
}