﻿using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using FluentValidation.TestHelper;

namespace Cypherly.ChatServer.Application.Test.Unit.CommandTest.ConnectClientCommandTest;

public class ConnectClientCommandValidatorTest
{
    private readonly ConnectClientCommandValidator _validator = new ConnectClientCommandValidator();

    [Fact]
    public void Should_Have_Error_When_ClientId_Is_Empty()
    {
        // Arrange
        var command = new ConnectClientCommand { ClientId = Guid.Empty, TransientId = "validTransientId" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Fact]
    public void Should_Have_Error_When_TransientId_Is_Empty()
    {
        // Arrange
        var command = new ConnectClientCommand { ClientId = Guid.NewGuid(), TransientId = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TransientId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new ConnectClientCommand { ClientId = Guid.NewGuid(), TransientId = "validTransientId" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ClientId);
        result.ShouldNotHaveValidationErrorFor(x => x.TransientId);
    }
}