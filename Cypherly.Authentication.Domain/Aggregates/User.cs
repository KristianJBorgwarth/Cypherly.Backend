﻿using System.Diagnostics.CodeAnalysis;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Aggregates;

[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class User : AggregateRoot
{
    public Email Email { get; init; } = null!;
    public Password Password { get; private set; } = null!;
    public bool IsVerified { get; private set; }

    private readonly List<UserVerificationCode> _verificationCodes = [];
    private readonly List<Device> _devices = [];
    public virtual IReadOnlyCollection<Device> Devices => _devices;
    public virtual IReadOnlyCollection<UserVerificationCode> VerificationCodes => _verificationCodes;
    public virtual IReadOnlyCollection<UserClaim> UserClaims { get; private set; } = new List<UserClaim>();
    public User() : base(Guid.Empty) { } // For EF Core

    public User(Guid id, Email email, Password password, bool isVerified) : base(id)
    {
        Email = email;
        Password = password;
        IsVerified = isVerified;
    }

    /// <summary>
    /// Adds a verification code to the user for the specified code type.
    /// <para>
    /// Marks any existing codes of the same type as used.
    /// </para>
    /// </summary>
    /// <param name="codeType"><see cref="UserVerificationCodeType"/></param>
    public void AddVerificationCode(UserVerificationCodeType codeType)
    {
        var existingCodes = VerificationCodes.Where(vc => vc.CodeType == codeType && !vc.Code.IsUsed);

        foreach (var vc in existingCodes)
        {
            vc.Code.Use();
        }

        _verificationCodes.Add(new(Guid.NewGuid(), userId: Id, codeType));
    }

    /// <summary>
    /// Returns the most recent active verification code of the specified type.
    /// <para>
    /// Returns null if no active codes are found.
    /// </para>
    /// </summary>
    /// <param name="codeType"><see cref="UserVerificationCodeType"/></param>
    /// <returns></returns>
    public UserVerificationCode? GetActiveVerificationCode(UserVerificationCodeType codeType)
    {
        return VerificationCodes.Where(vc => vc.CodeType == codeType && !vc.Code.IsUsed && vc.Code.ExpirationDate > DateTime.UtcNow).MaxBy(vc => vc.Code.ExpirationDate);
    }

    /// <summary>
    /// Checks the count of the verification codes and verifies the user if the code is valid.
    /// </summary>
    /// <param name="verificationCode">Value representing the VerificationCode.Code <see cref="UserVerificationCode.Code"/></param>
    /// <returns>Result representing the verification result</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Result Verify(string verificationCode)
    {
        if (VerificationCodes.Count == 0)
            throw new InvalidOperationException("This chat user does not have a verification code");

        if (IsVerified)
            throw new InvalidOperationException("This chat user is already verified");

        var userVerificationCode = VerificationCodes.FirstOrDefault(uvc => uvc.Code.Value == verificationCode);
        if (userVerificationCode is null) return Result.Fail(Errors.General.UnspecifiedError("Invalid verification code"));

        var verificationResult = userVerificationCode.Code.Verify(verificationCode);

        if (verificationResult.Success is false)
            return verificationResult;

        userVerificationCode.Code.Use();
        IsVerified = true;
        AddDomainEvent(new UserVerifiedEvent(Id));
        return Result.Ok();
    }

    /// <summary>
    /// Get the user claims for the user.
    /// </summary>
    /// <returns>list of <see cref="UserClaim"/></returns>
    public List<UserClaim> GetUserClaims()
    {
        return UserClaims.ToList();
    }

    public void AddDevice(Device device)
    {
        _devices.Add(device);
    }

    public Device GetDevice(Guid deviceId)
    {
        return Devices.FirstOrDefault(d => d.Id == deviceId) ?? throw new InvalidOperationException("Device not found");
    }

    public List<Device> GetValidDevices()
    {
        return Devices.Where(d => d.Status == DeviceStatus.Trusted).ToList();
    }
}