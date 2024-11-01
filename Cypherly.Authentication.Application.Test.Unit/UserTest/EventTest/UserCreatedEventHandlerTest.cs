﻿using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Events;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.EventTest;

public class UserCreatedEventHandlerTest
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly IProducer<SendEmailMessage> _fakeEmailProducer;
    private readonly UserCreatedEventHandler _sut;

    public UserCreatedEventHandlerTest()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        _fakeEmailProducer = A.Fake<IProducer<SendEmailMessage>>();
        var fakeLogger = A.Fake<ILogger<UserCreatedEventHandler>>();

        _sut = new(_fakeUserRepository, _fakeEmailProducer, fakeLogger);
    }

    [Fact]
    public async void Handle_UserCreatedEvent_UserFound_SendEmailMessage()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Test=??8239"), false);
        user.AddVerificationCode(VerificationCodeType.EmailVerification);

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).Returns(user);

        A.CallTo(()=> _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);

        var userCreatedEvent = new UserCreatedEvent(user.Id);

        // Act
        await _sut.Handle(userCreatedEvent, default);

        // Assert
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_UserCreatedEvent_UserNotFound_ThrowException()
    {
        // Arrange
        var userCreatedEvent = new UserCreatedEvent(Guid.NewGuid());
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(userCreatedEvent.UserId)).Returns<User?>(null);

        // Act
        Func<Task> act = async () => await _sut.Handle(userCreatedEvent, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("User not found");
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(userCreatedEvent.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_UserCreatedEvent_VerificationCodeNotFound_ThrowException()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("fuckasdk?2329JS"), false);
        var userEvent = new UserCreatedEvent(user.Id);

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).Returns(user);

        //Will throw since user will have no verification code
        //Act
        Func<Task> act = async () => await _sut.Handle(userEvent, default);


        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Verification code not found");

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(userEvent.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_UserCreatedEvent_SendEmailMessageFailed_ThrowException()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Test=??8239"), false);
        user.AddVerificationCode(VerificationCodeType.EmailVerification);

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).Returns(user);

        A.CallTo(() => _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .Throws<Exception>();

        var userCreatedEvent = new UserCreatedEvent(user.Id);

        // Act
        Func<Task> act = async () => await _sut.Handle(userCreatedEvent, default);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(user.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeEmailProducer.PublishMessageAsync(A<SendEmailMessage>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}