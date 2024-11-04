using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.AuthenticationTest.Login;

public class LoginCommandHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly IJwtService _fakeJwtService;
    private readonly IAuthenticationService _fakeAuthService;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly ILogger<LoginCommandHandler> _fakeLogger;
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeJwtService = A.Fake<IJwtService>();
        _fakeAuthService = A.Fake<IAuthenticationService>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeLogger = A.Fake<ILogger<LoginCommandHandler>>();
        _sut = new LoginCommandHandler(_fakeRepo, _fakeJwtService, _fakeAuthService, _fakeUnitOfWork, _fakeLogger);
    }
    [Fact]
    public async Task Handle_Command_When_Valid_Returns_Ok()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.com"), Password.Create("PÅ23?assword"), true);

        var cmd = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "PÅ23?assword"
        };
        A.CallTo(()=> _fakeRepo.GetByEmailAsync(user.Email.Address)).Returns(user);
        A.CallTo(()=> _fakeJwtService.GenerateToken(user.Id, user.Email.Address, user.GetUserClaims())).Returns("token");
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(user)).Returns(new RefreshToken(Guid.NewGuid(),user.Id));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.JwtToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshTokenExpires.Should().NotBe(DateTime.MinValue);
    }

    [Fact]
    public async Task Handle_Command_When_Email_Is_Invalid_Returns_Fail()
    {
        // Arrange
        var cmd = new LoginCommand
        {
            Email = "lol",
            Password = "PÅ23?assword"
        };

        A.CallTo(()=> _fakeRepo.GetByEmailAsync(cmd.Email))!.Returns<User>(null!);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Invalid Credentials");
        A.CallTo(()=>_fakeJwtService.GenerateToken(A<Guid>._, A<string>._, new())).MustNotHaveHappened();
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(A<User>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Command_When_Password_Is_Invalid_Returns_Fail()
    {
        // Arrange
        var cmd = new LoginCommand
        {
            Email = "lol",
            Password = "PÅ23?assword"
        };

        A.CallTo(()=>_fakeRepo.GetByEmailAsync(cmd.Email)).Returns(new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?23"), true));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Invalid Credentials");
        A.CallTo(()=>_fakeJwtService.GenerateToken(A<Guid>._, A<string>._, new())).MustNotHaveHappened();
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(A<User>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Command_When_User_Is_Not_Verified_Returns_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.com"), Password.Create("PÅ23?assword"), false); // User is not verified

        var cmd = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "PÅ23?assword"
        };
        A.CallTo(()=> _fakeRepo.GetByEmailAsync(user.Email.Address)).Returns(user);
        A.CallTo(()=> _fakeJwtService.GenerateToken(user.Id, user.Email.Address, user.GetUserClaims())).Returns("token");
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(user)).Returns(new RefreshToken(Guid.NewGuid(),user.Id));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsVerified.Should().BeFalse();
        result.Value.JwtToken.Should().BeNull();
        result.Value.RefreshToken.Should().BeNull();
        result.Value.RefreshTokenExpires.Should().BeNull();
        A.CallTo(()=>_fakeJwtService.GenerateToken(A<Guid>._, A<string>._, new())).MustNotHaveHappened();
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(A<User>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Command_When_Exception_Occurs_Returns_Fail()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@mail.dk",
            Password = "TestPassword?23"
        };

        A.CallTo(()=> _fakeRepo.GetByEmailAsync(command.Email)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("An exception occured while attempting to login");
        A.CallTo(()=>_fakeJwtService.GenerateToken(A<Guid>._, A<string>._, new())).MustNotHaveHappened();
        A.CallTo(()=>_fakeAuthService.GenerateRefreshToken(A<User>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
    }

}