// ReSharper disable InconsistentNaming
namespace Cypherly.Authentication.Domain.Enums;

public enum DeviceType
{
    Mobile = 1,
    Desktop = 2,
}

public enum DeviceStatus
{
    Active = 1,
    Trusted = 2,
    Inactive = 3,
    Pending = 4,
    Revoked = 5,
}

public enum DevicePlatform
{
    Windows = 1,
    iOS = 2,
    Android = 3,
    MacOS = 4,
    Linux = 5,
    Unknown = 6,
}

