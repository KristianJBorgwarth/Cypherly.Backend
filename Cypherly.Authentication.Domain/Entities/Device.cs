using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class Device : Entity
{
    public string Name { get; init; } = null!;
    public string PublicKey { get; init; } = null!;
    public DeviceStatus Status { get; private set; }
    public DeviceType? Type { get; init; }
    public DevicePlatform? Platform { get; init; }
    public string AppVersion { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    private readonly List<RefreshToken> _refreshTokens = [];
    public virtual IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    public Device() : base(Guid.Empty) {} // For EF Core

    public Device(Guid id,
        string name,
        string publicKey,
        string appVersion,
        DeviceType? type,
        DevicePlatform? platform,
        Guid userId) : base(id)
    {
        Name = name;
        PublicKey = publicKey;
        AppVersion = appVersion;
        UserId = userId;
        Type = type;
        Platform = platform;
        Status = DeviceStatus.Pending;
    }

    /// <summary>
    /// Set the status of the device to any of the <see cref="DeviceStatus"/> values.
    /// </summary>
    /// <param name="status"><see cref="DeviceStatus"/></param>
    public void SetStatus(DeviceStatus status) => Status = status;

    /// <summary>
    /// Adds a valid refresh token <see cref="RefreshToken"/> to the device.
    /// </summary>
    public void AddRefreshToken()
    {
        _refreshTokens.Add(new RefreshToken(Guid.NewGuid(), deviceId: Id));
    }

    /// <summary>
    /// Returns the most recent active refresh token.
    /// </summary>
    /// <returns><see cref="RefreshToken"/></returns>
    public RefreshToken? GetActiveRefreshToken()
    {
        return RefreshTokens.Where(rt=> rt.IsValid()).MaxBy(rt => rt.Expires);
    }

}