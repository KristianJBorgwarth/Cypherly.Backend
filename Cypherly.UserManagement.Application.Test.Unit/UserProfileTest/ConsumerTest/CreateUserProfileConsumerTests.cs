using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Consumers;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using MassTransit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.ConsumerTest
{
    public class CreateUserProfileConsumerTest
    {
        private readonly IUserProfileRepository _fakeUserProfileRepo;
        private readonly IUnitOfWork _fakeUnitOfWork;
        private readonly IUserProfileService _fakeUserProfileService;
        private readonly CreateUserProfileConsumer _sut;

        public CreateUserProfileConsumerTest()
        {
            _fakeUserProfileRepo = A.Fake<IUserProfileRepository>();
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();
            _fakeUserProfileService = A.Fake<IUserProfileService>();
            var fakeLogger = A.Fake<ILogger<CreateUserProfileConsumer>>();
            _sut = new(_fakeUserProfileRepo, _fakeUnitOfWork, _fakeUserProfileService, fakeLogger);
        }

        [Fact]
        public async Task Consume_WhenCalled_ShouldCreateUserProfile()
        {
            // Arrange
            var message = new CreateUserProfileRequest(Guid.NewGuid(), "TestUser");

            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            A.CallTo(() => _fakeUserProfileService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);

            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Returns(Task.CompletedTask);

            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileRequest>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            // Act
            await _sut.Consume(fakeConsumeContext);

            // Assert
            A.CallTo(() => _fakeUserProfileService.CreateUserProfile(message.UserId, message.Username)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Consume_WhenServiceThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = new CreateUserProfileRequest(Guid.NewGuid(), "TestUser");

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileRequest>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileService.CreateUserProfile(message.UserId, message.Username))
                .Throws(new Exception("Service exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }

        [Fact]
        public async Task Consume_WhenRepositoryThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = new CreateUserProfileRequest(Guid.NewGuid(), "TestUser");
            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileRequest>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Throws(new Exception("Repository exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }

        [Fact]
        public async Task Consume_WhenUnitOfWorkThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = new CreateUserProfileRequest(Guid.NewGuid(), "TestUser");
            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileRequest>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Returns(Task.CompletedTask);
            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).Throws(new Exception("UnitOfWork exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }
    }
}
