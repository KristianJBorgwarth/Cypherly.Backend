using Cypherly.Application.Contracts.Messaging.PublishMessages;
using Cypherly.Application.Contracts.Messaging.PublishMessages.Email;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Events;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.EventTest;

public class VerificationCodeGeneratedEventHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly IProducer<SendEmailMessage> _fakeEmailProducer;
    private readonly VerificationCodeGeneratedEventHandler _sut;

    public VerificationCodeGeneratedEventHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeEmailProducer = A.Fake<IProducer<SendEmailMessage>>();
        var fakeLogger = A.Fake<ILogger<VerificationCodeGeneratedEventHandler>>();
        _sut = new(_fakeRepo, _fakeEmailProducer, fakeLogger);
    }

    [Fact]
    public async Task Handle_Valid_Notification_Should_Send_Email()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kjsKidh??923"), false);
        user.AddVerificationCode(VerificationCodeType.EmailVerification);

        var notification = new VerificationCodeGeneratedEvent(user.Id, VerificationCodeType.EmailVerification);

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);
        A.CallTo(()=> _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, default)).DoesNothing();

        // Act
        await _sut.Handle(notification, default);

        // Assert
        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Notification_With_Invalid_Id_Should_Throw_Exception()
    {
        // Arrange
        var notification = new VerificationCodeGeneratedEvent(Guid.NewGuid(), VerificationCodeType.EmailVerification);
        A.CallTo(() => _fakeRepo.GetByIdAsync(notification.UserId)).Returns<User?>(null);


        // Act
        var act = async () => await _sut.Handle(notification, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        A.CallTo(() => _fakeRepo.GetByIdAsync(notification.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, default)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Notification_With_Missing_Verification_Code_Should_Throw_Exception()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kjsKidh??923"), false);

        var notification = new VerificationCodeGeneratedEvent(user.Id, VerificationCodeType.EmailVerification);

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        // Act
        var act = async () => await _sut.Handle(notification, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, default)).MustNotHaveHappened();
    }

}