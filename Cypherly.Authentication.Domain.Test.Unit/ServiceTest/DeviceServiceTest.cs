using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FluentAssertions;

namespace Cypherly.Authentication.Domain.Test.Unit.ServiceTest;

public class DeviceServiceTest
{
    private readonly IDeviceService _sut = new DeviceService();

    [Fact]
    public void RegisterDevice_Should_Return_Device()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Testpass?932"), true);

        // Act
        var result = _sut.RegisterDevice(user, "publicKey", "1.0.0", DeviceType.Mobile, DevicePlatform.iOS);

        // Assert
        result.Should().NotBeNull();
        user.Devices.Should().HaveCount(1);
    }
}