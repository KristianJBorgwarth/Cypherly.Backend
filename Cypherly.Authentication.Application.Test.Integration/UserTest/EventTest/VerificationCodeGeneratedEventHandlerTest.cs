using Cypherly.Application.Contracts.Messaging.PublishMessages;
using Cypherly.Application.Contracts.Messaging.PublishMessages.Email;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Events;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EventTest;

public class VerificationCodeGeneratedEventHandlerTest : IntegrationTestBase
{
    private readonly VerificationCodeGeneratedEventHandler _sut;
    public VerificationCodeGeneratedEventHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var emailProducer = scope.ServiceProvider.GetRequiredService<IProducer<SendEmailMessage>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<VerificationCodeGeneratedEventHandler>>();

        _sut = new(userRepository, emailProducer, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Notification_Should_Send_Email()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("testPassword?923"), false);
        user.AddVerificationCode(VerificationCodeType.EmailVerification);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var notification = new VerificationCodeGeneratedEvent(user.Id, VerificationCodeType.EmailVerification);

        // Act
        await _sut.Handle(notification, default);

        // Assert
        Harness.Published.Select<SendEmailMessage>().FirstOrDefault(uc => uc.Context.Message.To == user.Email.Address)
            .Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Given_Invalid_Notification_Should_Throw_Exception()
    {
        // Arrange
        var notification = new VerificationCodeGeneratedEvent(Guid.NewGuid(), VerificationCodeType.EmailVerification);

        // Act
        Func<Task> act = async () => await _sut.Handle(notification, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}