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

public class UserCreatedEventHandlerTest : IntegrationTestBase
{
    private readonly UserCreatedEventHandler _sut;
    public UserCreatedEventHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var emailProducer = scope.ServiceProvider.GetRequiredService<IProducer<SendEmailMessage>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserCreatedEventHandler>>();
        _sut = new(userRepository, emailProducer, logger);
    }

    [Fact]
    public async void Handle_Given_Valid_Event_Should_Send_Email()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("validPassword=?23"), false);
        user.AddVerificationCode(VerificationCodeType.EmailVerification);
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var @event = new UserCreatedEvent(user.Id);

        // Act
        await _sut.Handle(@event, CancellationToken.None);

        // Assert
        Harness.Published.Select<SendEmailMessage>().FirstOrDefault(uc=>uc.Context.Message.To == user.Email.Address).Should().NotBeNull();
    }

    [Fact]
    public async void Handle_Given_Invalid_Event_Should_Throw_Exception()
    {
        // Arrange
        var @event = new UserCreatedEvent(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _sut.Handle(@event, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}