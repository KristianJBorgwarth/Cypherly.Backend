using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string devicePublicKey,
        string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform);
}

public class DeviceService : IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string devicePublicKey, string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform)
    {
        var device = new Device(Guid.NewGuid(), devicePublicKey, deviceAppVersion, deviceType, devicePlatform, user.Id);

        user.AddDevice(device);

        return device;
    }
}