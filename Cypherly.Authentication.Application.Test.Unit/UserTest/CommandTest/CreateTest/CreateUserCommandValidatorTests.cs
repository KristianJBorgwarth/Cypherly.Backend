﻿using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Domain.Common;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.CreateTest;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _sut = new();

    [Fact]
    public void Validate_Given_Valid_Command_Should_Return_Valid()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "valid@email.dk",
            Password = "validPassword97?"
        };
        
        // Act
        var result = _sut.TestValidate(cmd);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Given_Empty_Email_Should_Return_Invalid()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "",
            Password = "validPassword97?"
        };
        
        // Act 
        var result = _sut.TestValidate(cmd);
        
        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be(Errors.General.ValueIsEmpty(nameof(CreateUserCommand.Email)).Message);
    }

    [Fact]
    public void Validate_Given_Empty_Password_Should_Return_Invalid()
    {
        // Arrange
        var cmd = new CreateUserCommand()
        {
            Email = "valid@email.com",
            Password = "",
        };
        
        // Act
        var result = _sut.TestValidate(cmd);
        
        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should()
            .Be(Errors.General.ValueIsEmpty(nameof(CreateUserCommand.Password)).Message);
    }
}