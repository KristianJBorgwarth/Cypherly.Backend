using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.ServiceTest;

public class DeviceServiceTest
{
    private readonly IDeviceService sut = new DeviceService();

    [Fact]
    public void RegisterDevice_Should_Return_Device()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Testpass?932"), true);

        // Act
        var result = sut.RegisterDevice(user, "publicKey", "1.0.0", DeviceType.Mobile, DevicePlatform.iOS);

        // Assert
        result.Should().NotBeNull();
        user.Devices.Should().HaveCount(1);
        user.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void VerifyDevice_When_Code_Is_valid_Should_Return_ResultOk()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Testpass?932"), true);
        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, user.Id);
        device.AddDeviceVerificationCode();

        user.AddDevice(device);

        var verificationCode = device.GetActiveVerificationCode()!.Code.Value;

        // Act
        var result = sut.VerifyDevice(user, device.Id, verificationCode);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void VerifyDevice_When_Code_Is_Invalid_Should_Return_ResultError()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Testpass?932"), true);

        var device = new Device(Guid.NewGuid(), "publicKey", "1.0.0", DeviceType.Mobile,
            DevicePlatform.iOS, user.Id);

        device.AddDeviceVerificationCode();

        user.AddDevice(device);

        // Act
        var result = sut.VerifyDevice(user, device.Id, "1234");

        // Assert
        result.Success.Should().BeFalse();
    }
}