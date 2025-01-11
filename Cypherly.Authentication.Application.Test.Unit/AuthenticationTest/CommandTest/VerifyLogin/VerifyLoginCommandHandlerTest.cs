﻿using Castle.Components.DictionaryAdapter.Xml;
using Cypherly.Authentication.Application.Caching.LoginNonce;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyLogin;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.AuthenticationTest.CommandTest.VerifyLogin;

public class VerifyLoginCommandHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly ILoginNonceCache _fakeNonceCache;
    private readonly VerifyLoginCommandHandler _sut;

    public VerifyLoginCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeNonceCache = A.Fake<ILoginNonceCache>();
        var fakeLogger = A.Fake<ILogger<VerifyLoginCommandHandler>>();
        _sut = new VerifyLoginCommandHandler(_fakeRepo, _fakeNonceCache, fakeLogger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Return_LoginNonce_ResultOk()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kjash)023+23?JK"), true);
        user.AddVerificationCode(UserVerificationCodeType.Login);

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        var command = new VerifyLoginCommand()
        {
            UserId = user.Id,
            LoginVerificationCode = user.GetActiveVerificationCode(UserVerificationCodeType.Login)!.Code.Value,
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Nonce.Should().NotBeNullOrEmpty();
        A.CallTo(()=> _fakeNonceCache.AddNonceAsync(A<LoginNonce>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Repo_Returns_Null_Should_Return_ResultFail()
    {
        // Arrange
        A.CallTo(() => _fakeRepo.GetByIdAsync(A<Guid>._)).Returns((User)null!);

        var command = new VerifyLoginCommand()
        {
            UserId = Guid.NewGuid(),
            LoginVerificationCode = "123456",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        A.CallTo(()=> _fakeNonceCache.AddNonceAsync(A<LoginNonce>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Invalid_VerificationCode_Should_Return_ResultFail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kjash)023+23?JK"), true);
        user.AddVerificationCode(UserVerificationCodeType.Login);

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        var command = new VerifyLoginCommand()
        {
            UserId = user.Id,
            LoginVerificationCode = "invalid code",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Invalid verification code");
        A.CallTo(()=> _fakeNonceCache.AddNonceAsync(A<LoginNonce>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Something_Throws_Exception_Should_Return_ResultFail()
    {
        // Arrange
        A.CallTo(() => _fakeRepo.GetByIdAsync(A<Guid>._)).Throws<Exception>();

        var command = new VerifyLoginCommand()
        {
            UserId = Guid.NewGuid(),
            LoginVerificationCode = "123456",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occurred while verifying login");
        A.CallTo(() => _fakeNonceCache.AddNonceAsync(A<LoginNonce>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}