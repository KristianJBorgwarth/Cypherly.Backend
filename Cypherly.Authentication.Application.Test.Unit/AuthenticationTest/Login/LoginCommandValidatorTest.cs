using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using FluentValidation.TestHelper;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.AuthenticationTest.Login;

public class LoginCommandValidatorTest
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "",
            Password = "ValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("The value cannot be empty: Email "); // Or the actual message you defined
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = new string('a', 256),
            Password = "ValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Value 'Email' should not exceed 255."); // Or the actual message you defined
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("The value cannot be empty: Password "); // Or the actual message you defined
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = new('a', 256)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Value 'Password' should not exceed 255."); // Or the actual message you defined
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}