using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Consumers;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.ConsumerTest;

public class CreateUserProfileConsumerTest : IntegrationTestBase
{
    private readonly CreateUserProfileConsumer _sut;
    public CreateUserProfileConsumerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileLifecycleService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateUserProfileConsumer>>();
        _sut = new(userProfileRepository, unitOfWork, userProfileService, logger);
    }

    [Fact]
    public async void Consume_ValidRequest_Should_Create_UserProfile()
    {
        // Arrange
        var request = new CreateUserProfileRequest(Guid.NewGuid(), "testUser");


        var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileRequest>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(request);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.Count().Should().Be(1);
        Db.UserProfile.First().Id.Should().Be(request.UserId);
        Db.UserProfile.First().Username.Should().Be(request.Username);
        Db.UserProfile.First().ConnectionId.Should().NotBe(Guid.Empty);
    }
}