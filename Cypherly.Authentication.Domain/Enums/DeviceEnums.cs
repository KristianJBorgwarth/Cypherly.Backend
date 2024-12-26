// ReSharper disable InconsistentNaming
namespace Cypherly.Authentication.Domain.Enums;

public enum DeviceType
{
    Mobile,
    Desktop,
}

public enum DeviceStatus
{
    Active,
    Trusted,
    Inactive,
    Pending,
    Revoked,
}

public enum DevicePlatform
{
    iOS,
    Android,
    Windows,
    MacOS,
    Linux,
    Unknown,
}

