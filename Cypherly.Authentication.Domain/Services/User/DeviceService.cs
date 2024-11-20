using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey,
        string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform);

    public Result VerifyDevice(Aggregates.User user, Guid deviceId, string deviceVerificationCode);

    public bool DoesTrustedDeviceExist(Aggregates.User user);
}

public class DeviceService : IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey, string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform)
    {
        var device = new Device(Guid.NewGuid(), deviceName, devicePublicKey, deviceAppVersion, deviceType, devicePlatform, user.Id);

        device.AddDeviceVerificationCode();

        user.AddDevice(device);

        user.AddDomainEvent(new DeviceCreatedEvent(user.Id, device.VerificationCodes.First().Code.Value));

        return device;
    }

    public Result VerifyDevice(Aggregates.User user, Guid deviceId, string deviceVerificationCode)
    {
        var device = user.GetDevice(deviceId);

        var verifyDeviceResult = device.Verify(deviceVerificationCode);

        if (verifyDeviceResult.Success is false)
            return verifyDeviceResult;

        return Result.Ok();
    }

    public bool DoesTrustedDeviceExist(Aggregates.User user)
    {
        return user.Devices.Any(x => x.Status == DeviceStatus.Trusted);
    }
}