using Cypherly.Authentication.Domain.Entities;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.EntityTest;

public class VerificationCodeTest
{
    [Fact]
    public void Verify_WhenCodeIsUsed_ReturnsError()
    {
        // Arrange
        var verificationCode = new VerificationCode(Guid.NewGuid(),Guid.NewGuid());
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
        var verificationCode = new VerificationCode(Guid.NewGuid(), Guid.NewGuid());

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
        var verificationCode = new VerificationCode(Guid.NewGuid(),Guid.NewGuid());

        // Act
        var result = verificationCode.Verify(verificationCode.Code);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Use_MarksCodeAsUsed()
    {
        // Arrange
        var verificationCode = new VerificationCode(Guid.NewGuid(),Guid.NewGuid());

        // Act
        verificationCode.Use();

        // Assert
        verificationCode.IsUsed.Should().BeTrue();
    }
}