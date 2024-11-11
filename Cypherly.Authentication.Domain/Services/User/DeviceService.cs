using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey, string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform);
}

public class DeviceService : IDeviceService
{
    /// <summary>
    /// Adds a new device to the user's account.
    /// If the user has no devices (i.e. this is the users first device), the new device will be set to active status automatically.
    /// </summary>
    /// <param name="user">the user to whom the device will be registered</param>
    /// <param name="deviceName">the name of the device</param>
    /// <param name="devicePublicKey">the public key for authentication of the device</param>
    /// <param name="deviceAppVersion">the current version of the app registered on the device</param>
    /// <param name="deviceType">the type of device <see cref="DeviceType"/></param>
    /// <param name="devicePlatform">the platform of the device <see cref="DevicePlatform"/></param>
    /// <returns></returns>
    public Device RegisterDevice(Aggregates.User user, string deviceName, string devicePublicKey, string deviceAppVersion,
        DeviceType deviceType, DevicePlatform devicePlatform)
    {
        var device = new Device(Guid.NewGuid(), deviceName, devicePublicKey, deviceAppVersion, deviceType,
            devicePlatform, user.Id);

        if(user.Devices.Count == 0)
            device.SetStatus(DeviceStatus.Active);
        else
        {
            //TODO: add new device event here
        }

        user.AddDevice(device);

        return device;
    }
}