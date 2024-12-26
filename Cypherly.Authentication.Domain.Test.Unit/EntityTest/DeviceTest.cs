using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.EntityTest;

public class DeviceTest
{
    [Fact]
    public void AddDeviceVerificationCode_ShouldAddVerificationCode()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        device.AddDeviceVerificationCode();

        // Assert
        device.VerificationCodes.Should().HaveCount(1);
    }

    [Fact]
    public void GetActiveVerificationCode_ShouldReturnNull_WhenNoVerificationCode()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        var result = device.GetActiveVerificationCode();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetActiveVerificationCode_ShouldReturnVerificationCode_WhenVerificationCodeExists()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());
        device.AddDeviceVerificationCode();

        // Act
        var result = device.GetActiveVerificationCode();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Verify_ShouldThrowInvalidOperationException_WhenNoVerificationCode()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        Action act = () => device.Verify("1234");

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("This device does not have a verification code");
    }

    [Fact]
    public void Verify_ShouldReturnSuccess_WhenVerificationCodeIsValid()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());
        device.AddDeviceVerificationCode();


        var verificationCode = device.GetActiveVerificationCode()!.Code.Value;

        // Act
        var result = device.Verify(verificationCode);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldThrowInvalidOperationException_WhenDeviceIsTrusted()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());
        device.AddDeviceVerificationCode();
        device.Verify(device.GetActiveVerificationCode()!.Code.Value);

        // Act
        Action act = () => device.Verify("1234");

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("This device is already trusted");
    }
    [Fact]
    public void Verify_ShouldReturnFailure_WhenVerificationCodeIsInvalid()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(),  "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());
        device.AddDeviceVerificationCode();

        // Act
        var result = device.Verify("1234");

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(DeviceStatus.Active)]
    [InlineData(DeviceStatus.Trusted)]
    [InlineData(DeviceStatus.Inactive)]
    public async void SetStatus_ShouldSetStatus(DeviceStatus status)
    {
        // Arrange
        var device = new Device(Guid.NewGuid(),"publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        device.SetStatus(status);

        // Assert
        device.Status.Should().Be(status);
    }

    [Fact]
    public void AddRefreshToken_Should_AddRefreshToken()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        device.AddRefreshToken();

        // Assert
        device.RefreshTokens.Should().HaveCount(1);
    }

    [Fact]
    public void GetActiveRefreshToken_Should_Return_Null_When_No_Token()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(),"publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        // Act
        var result = device.GetActiveRefreshToken();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetActiveRefreshToken_Should_Return_Token_When_Token_Exists()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, Guid.NewGuid());

        device.AddRefreshToken();

        // Act
        var result = device.GetActiveRefreshToken();

        // Assert
        result.Should().NotBeNull();
    }
}