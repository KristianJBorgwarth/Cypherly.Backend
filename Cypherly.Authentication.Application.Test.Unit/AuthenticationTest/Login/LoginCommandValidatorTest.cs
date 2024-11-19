using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using Cypherly.Authentication.Domain.Enums;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Cypherly.Authentication.Application.Test.Unit.AuthenticationTest.Login;

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
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("The value cannot be empty: Email ");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = new string('a', 256),
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Value 'Email' should not exceed 255.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("The value cannot be empty: Password ");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = new('a', 256),
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Value 'Password' should not exceed 255.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Have_Error_When_DeviceName_Is_Too_Short()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "A",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceName)
            .WithErrorMessage("Value 'DeviceName' should be at least 3.");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceName_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = new string('a', 41),
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceName)
            .WithErrorMessage("Value 'DeviceName' should not exceed 40.");
    }

    [Fact]
    public void Should_Have_Error_When_Base64DevicePublicKey_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = new string('a', 101),
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base64DevicePublicKey)
            .WithErrorMessage("Value 'Base64DevicePublicKey' should not exceed 100.");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceAppVersion_Exceeds_Max_Length()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "123.456",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceAppVersion)
            .WithErrorMessage("Value 'DeviceAppVersion' should not exceed 6.");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceAppVersion_Is_Invalid_Format()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1..0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceAppVersion)
            .WithErrorMessage("Value 'DeviceAppVersion' is not valid in this context");
    }

    [Fact]
    public void Should_Not_Have_Error_When_DeviceAppVersion_Is_Valid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Have_Error_When_DeviceType_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = default,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceType)
            .WithErrorMessage("The value cannot be empty: DeviceType ");
    }

    [Fact]
    public void Should_Have_Error_When_DevicePlatform_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "ValidPassword123!",
            DeviceName = "TestDevice",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = default
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DevicePlatform)
            .WithErrorMessage("The value cannot be empty: DevicePlatform ");
    }



}