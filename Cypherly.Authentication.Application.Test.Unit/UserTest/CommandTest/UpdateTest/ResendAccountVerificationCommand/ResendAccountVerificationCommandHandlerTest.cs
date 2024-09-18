using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.UpdateTest.ResendAccountVerificationCommand;

public class ResendAccountVerificationCommandHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly IUserService _fakeUserService;
    private readonly ResendAccountVerificationCommandHandler _sut;

    public ResendAccountVerificationCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeUserService = A.Fake<IUserService>();
        var fakeLogger = A.Fake<ILogger<ResendAccountVerificationCommandHandler>>();
        _sut = new ResendAccountVerificationCommandHandler(_fakeRepo, _fakeUnitOfWork, _fakeUserService, fakeLogger);
    }

    [Fact]
    public async Task Handle_Command_With_Invalid_Id_Should_Return_Result_Fail()
    {
        // Arrange
        var cmd = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
        {
            UserId = Guid.NewGuid()
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserId))!.Returns<User>(null);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_Command_With_User_Verified_Should_Return_Result_Fail()
    {
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kjsKidh??923"), true);
        var cmd = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
        {
            UserId = user.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserId)).Returns(user);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("User is already verified");
    }

    [Fact]
    public async Task Handle_Command_When_Something_Throws_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kjsKidh??923"), false);
        var cmd = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
        {
            UserId = user.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserId)).Returns(user);
        A.CallTo(() => _fakeUserService.GenerateVerificationCode(user, VerificationCodeType.EmailVerification)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An exception occurred while resending verification code for user");
    }

    [Fact]
    public async Task Handle_Command_With_Valid_Id_Should_Generate_New_Verification_Code_And_Return_Result_Ok()
    {

        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kjsKidh??923"), false);
        var cmd = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
        {
            UserId = user.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserId)).Returns(user);
        A.CallTo(()=> _fakeUserService.GenerateVerificationCode(user, VerificationCodeType.EmailVerification)).DoesNothing();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).DoesNothing();

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.Success.Should().BeTrue();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(cmd.UserId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUserService.GenerateVerificationCode(user, VerificationCodeType.EmailVerification)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();

    }
}