using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.ValueObjects;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.AggregateRootTest;

public class UserTest
{
    [Fact]
    public void SetVerificationCode_Should_Set_VerificationCode()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: false);

        // Act
        user.SetVerificationCode();

        // Assert
        user.VerificationCode.Should().NotBeNull();
        user.VerificationCode.Code.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Verify_Should_Fail_When_No_VerificationCode_Is_Set()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: false);

        // Act
        var result = user.Verify("someCode");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("This chat user does not have a verification code");
    }

    [Fact]
    public void Verify_Should_Fail_When_User_Is_Already_Verified()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: true);
        user.SetVerificationCode();

        // Act
        var result = user.Verify(user.VerificationCode!.Code);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("This chat user is already verified");
    }

    [Fact]
    public void Verify_Should_Fail_When_VerificationCode_Is_Invalid()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: false);
        user.SetVerificationCode();

        // Act
        var result = user.Verify("invalidCode");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Invalid verification code");
    }

    [Fact]
    public void Verify_Should_Succeed_When_VerificationCode_Is_Valid()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: false);
        user.SetVerificationCode();

        // Act
        var result = user.Verify(user.VerificationCode!.Code);

        // Assert
        result.Success.Should().BeTrue();
        user.IsVerified.Should().BeTrue();
        user.VerificationCode.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void Verify_Should_Fail_When_VerificationCode_Is_Already_Used()
    {
        // Arrange
        var email = Email.Create("test@mail.com").Value;
        var password = Password.Create("Password123!").Value;
        var user = new User(Guid.NewGuid(),email, password, isVerified: false);
        user.SetVerificationCode();

        // First time verification
        user.Verify(user.VerificationCode!.Code);

        // Act - try verifying again with the same code
        var result = user.Verify(user.VerificationCode.Code);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("This chat user is already verified");
    }
}
