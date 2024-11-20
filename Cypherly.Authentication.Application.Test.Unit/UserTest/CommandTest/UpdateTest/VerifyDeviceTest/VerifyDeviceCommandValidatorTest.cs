using Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.UpdateTest.VerifyDeviceTest;

public class VerifyDeviceCommandValidatorTest
{
    private readonly VerifyDeviceCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.Empty,
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = "ValidCode123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("The value cannot be empty: UserId ");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceId_Is_Empty()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.Empty,
            DeviceVerificationCode = "ValidCode123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceId)
            .WithErrorMessage("The value cannot be empty: DeviceId ");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceVerificationCode_Is_Empty()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceVerificationCode)
            .WithErrorMessage("The value cannot be empty: DeviceVerificationCode ");
    }

    [Fact]
    public void Should_Have_Error_When_DeviceVerificationCode_Exceeds_Max_Length()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = new string('a', 31) // 31 characters
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceVerificationCode)
            .WithErrorMessage("Value 'DeviceVerificationCode' should not exceed 30.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = "ValidCode123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
