using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Enums;
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

public class RollbackUserProfileDeleteConsumerTest : IntegrationTestBase
{
    private readonly RollbackUserProfileDeleteConsumer _sut;

    public RollbackUserProfileDeleteConsumerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RollbackUserProfileDeleteConsumer>>();
        _sut = new(userProfileRepository, userProfileService, unitOfWork, logger);
    }

    [Fact]
    public async void Consume_Valid_Message_Should_RevertSoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        userProfile.SetDelete();
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var message = new UserDeleteFailedMessage(userProfile.Id, Guid.NewGuid(), null, ServiceType.UserManagementService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async void Consume_Invalid_Message_Should_Not_RevertSoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        userProfile.SetDelete();
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        //Invalid ID
        var message = new UserDeleteFailedMessage(Guid.NewGuid(), Guid.NewGuid(), null, ServiceType.UserManagementService);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().NotBeNull();
    }
}