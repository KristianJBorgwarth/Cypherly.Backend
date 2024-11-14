using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey,
        string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform);

    public bool DoesTrustedDeviceExist(Aggregates.User user);
}

public class DeviceService : IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey,
        string deviceAppVersion,
        DeviceType deviceType, DevicePlatform devicePlatform)
    {
        var device = new Device(Guid.NewGuid(), deviceName, devicePublicKey, deviceAppVersion, deviceType,
            devicePlatform, user.Id);

        user.AddDevice(device);

        return device;
    }

    public bool DoesTrustedDeviceExist(Aggregates.User user)
    {
        return user.Devices.Any(x => x.Status == DeviceStatus.Trusted);
    }
}