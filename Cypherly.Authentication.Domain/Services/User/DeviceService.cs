using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string devicePublicKey,
        string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform);

    public Result VerifyDevice(Aggregates.User user, Guid deviceId, string deviceVerificationCode);
}

public class DeviceService : IDeviceService
{
    public Device RegisterDevice(Aggregates.User user, string devicePublicKey, string deviceAppVersion, DeviceType deviceType, DevicePlatform devicePlatform)
    {
        var device = new Device(Guid.NewGuid(), devicePublicKey, deviceAppVersion, deviceType, devicePlatform, user.Id);

        device.AddDeviceVerificationCode();

        user.AddDevice(device);

        user.AddDomainEvent(new DeviceCreatedEvent(user.Id, device.VerificationCodes.First().Code.Value));

        return device;
    }

    public Result VerifyDevice(Aggregates.User user, Guid deviceId, string deviceVerificationCode)
    {
        var device = user.GetDevice(deviceId);

        var verifyDeviceResult = device.Verify(deviceVerificationCode);

        return verifyDeviceResult.Success is false ? verifyDeviceResult : Result.Ok();
    }
}