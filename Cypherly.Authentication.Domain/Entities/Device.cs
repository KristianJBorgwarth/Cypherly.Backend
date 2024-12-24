using Cypherly.Authentication.Domain.Enums;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class Device : Entity
{
    public string Name { get; init; } = null!;
    public string PublicKey { get; init; } = null!;
    public DeviceStatus Status { get; private set; }
    public DeviceType Type { get; init; }
    public DevicePlatform Platform { get; init; }
    public string AppVersion { get; private set; } = null!;
    public Guid UserId { get; private set; }

    private readonly List<DeviceVerificationCode> _verificationCodes = [];

    private readonly List<RefreshToken> _refreshTokens = [];
    public virtual IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;
    public virtual IReadOnlyCollection<DeviceVerificationCode> VerificationCodes => _verificationCodes;
    public Device() : base(Guid.Empty) {} // For EF Core

    public Device(Guid id,
        string name,
        string publicKey,
        string appVersion,
        DeviceType type,
        DevicePlatform platform,
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
    /// Adds a verification code to the device.
    /// </summary>
    public void AddDeviceVerificationCode()
    {
        _verificationCodes.Add(new(Guid.NewGuid(), deviceId: Id));
    }

    /// <summary>
    /// Returns the most recent active verification code.
    /// </summary>
    /// <returns><see cref="DeviceVerificationCode"/></returns>
    public DeviceVerificationCode? GetActiveVerificationCode()
    {
        return VerificationCodes.Where(dvc => !dvc.Code.IsUsed && dvc.Code.ExpirationDate > DateTime.UtcNow).MaxBy(dvc => dvc.Code.ExpirationDate);
    }

    public Result Verify(string verificationCode)
    {
        if(VerificationCodes.Count == 0)
            throw new InvalidOperationException("This device does not have a verification code");

        if(Status == DeviceStatus.Trusted)
            throw new InvalidOperationException("This device is already trusted");

        var deviceVerificationCode = VerificationCodes.FirstOrDefault(c => c.Code.Value == verificationCode);
        if (deviceVerificationCode is null) return Result.Fail(Errors.General.UnspecifiedError("Invalid verification code"));

        var verificationResult = deviceVerificationCode.Code.Verify(verificationCode);
        if (verificationResult.Success is false)
            return verificationResult;

        Status = DeviceStatus.Trusted;
        deviceVerificationCode.Code.Use();
        return Result.Ok();
    }

    /// <summary>
    /// Set the status of the device to any of the <see cref="DeviceStatus"/> values.
    /// </summary>
    /// <param name="status"><see cref="DeviceStatus"/></param>
    public void SetStatus(DeviceStatus status) => Status = status;

    /// <summary>
    /// Adds a valid refresh token <see cref="RefreshToken"/> to the device.
    /// </summary>
    public RefreshToken AddRefreshToken()
    {
        var token = new RefreshToken(Guid.NewGuid(), deviceId: Id);
        _refreshTokens.Add(token);
        return token;
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