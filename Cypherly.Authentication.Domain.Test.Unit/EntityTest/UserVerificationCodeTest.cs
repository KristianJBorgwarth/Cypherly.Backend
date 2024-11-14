using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.EntityTest;

public class UserVerificationCodeTest
{
    [Fact]
    public void Verify_WhenCodeIsUsed_ReturnsError()
    {
        // Arrange
        var verificationCode = new UserVerificationCode(Guid.NewGuid(),Guid.NewGuid(), UserVerificationCodeType.EmailVerification);
        verificationCode.Use();

        // Act
        var result = verificationCode.Verify("123456");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Verification code has already been used");
    }

    [Fact]
    public void Verify_WhenCodeIsInvalid_ReturnsError()
    {
        // Arrange
        var verificationCode = new UserVerificationCode(Guid.NewGuid(), Guid.NewGuid(), UserVerificationCodeType.EmailVerification);

        // Act
        var result = verificationCode.Verify("123456");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Invalid verification code");
    }

    [Fact]
    public void Verify_WhenCodeIsValid_ReturnsSuccess()
    {
        // Arrange
        var verificationCode = new UserVerificationCode(Guid.NewGuid(),Guid.NewGuid(), UserVerificationCodeType.EmailVerification);

        // Act
        var result = verificationCode.Verify(verificationCode.Code);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Use_MarksCodeAsUsed()
    {
        // Arrange
        var verificationCode = new UserVerificationCode(Guid.NewGuid(),Guid.NewGuid(), UserVerificationCodeType.EmailVerification);

        // Act
        verificationCode.Use();

        // Assert
        verificationCode.IsUsed.Should().BeTrue();
    }
}