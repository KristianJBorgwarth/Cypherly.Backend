﻿using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.CommandTest.UpdateTest;

public class ResendVerificationCodeCommandHandlerTest : IntegrationTestBase
{
    private readonly ResendVerificationCodeCommandHandler _sut;
    public ResendVerificationCodeCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var verificationCodeService = scope.ServiceProvider.GetRequiredService<IVerificationCodeService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ResendVerificationCodeCommandHandler>>();
        _sut = new(repo, unitOfWork, verificationCodeService, logger);
    }

    [Fact]
    public async Task Handle_Valid_Command_Should_Generate_New_Verification_Code()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kj9823HHj?"), false);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new ResendVerificationCodeCommand
        {
            UserId = user.Id,
            CodeType = UserVerificationCodeType.EmailVerification,
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.User.AsNoTracking().Include(user => user.VerificationCodes).First().VerificationCodes.Should().HaveCount(1);
        Db.User.AsNoTracking().Include(user => user.VerificationCodes).First().VerificationCodes.First().CodeType.Should().Be(UserVerificationCodeType.EmailVerification);
    }

    [Fact]
    public async Task Handle_Valid_Command_With_CodeType_Login_Should_Generate_New_verification_Code()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kj9823HHj?"), false);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new ResendVerificationCodeCommand
        {
            UserId = user.Id,
            CodeType = UserVerificationCodeType.Login,
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.User.AsNoTracking().Include(user => user.VerificationCodes).First().VerificationCodes.Should().HaveCount(1);
        Db.User.AsNoTracking().Include(user => user.VerificationCodes).First().VerificationCodes.First().CodeType.Should().Be(UserVerificationCodeType.Login);
    }

    [Fact]
    public async Task Handle_Command_With_Invalid_Id_Should_Return_Result_Fail()
    {
        // Arrange
        var cmd = new ResendVerificationCodeCommand
        {
            UserId = Guid.NewGuid(),
            CodeType = UserVerificationCodeType.EmailVerification,
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_User_With_Verified_Status_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kj9823HHj?"),
            true); // Verified status set to true

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new ResendVerificationCodeCommand
        {
            UserId = user.Id,
            CodeType = UserVerificationCodeType.EmailVerification,
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("User is already verified");
    }
}