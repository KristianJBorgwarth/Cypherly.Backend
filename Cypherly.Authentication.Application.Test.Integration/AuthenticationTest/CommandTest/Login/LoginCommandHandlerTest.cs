﻿using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.AuthenticationTest.CommandTest.Login;

public class LoginCommandHandlerTest : IntegrationTestBase
{
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope().ServiceProvider;
        var repo = scope.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.GetRequiredService<IUnitOfWork>();
        var authservice = scope.GetRequiredService<IAuthenticationService>();
        var logger = scope.GetRequiredService<ILogger<LoginCommandHandler>>();
        _sut = new LoginCommandHandler(repo, authservice, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Login_Command_Should_Generate_Token_And_Return_LoginDto()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), true);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "TestPassword?123",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        Db.VerificationCode.Should().HaveCount(1);
        result.Value.IsVerified.Should().BeTrue();
        result.Value.UserId.Should().NotBeEmpty().And.Be(user.Id);
    }

    [Fact]
    public async Task Handle_Command_Given_Invalid_Email_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), true);

        const string invalidEmail = "this will be invalid";
        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = invalidEmail,
            Password = "TestPassword?123",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Invalid Credentials");
        Db.RefreshToken.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_Command_Given_Invalid_Password_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), true);

        const string invalidPw = "this will be invalid";
        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = user.Email.Address,
            Password = invalidPw,
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Invalid Credentials");
    }

    [Fact]
    public async Task Handle_Command_Given_User_Not_Verified_Should_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), false); // Unverified user

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "TestPassword?123",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsVerified.Should().BeFalse();
        Db.RefreshToken.Should().HaveCount(0);
    }
}