using System.Diagnostics.CodeAnalysis;
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

    private readonly List<VerificationCode> _verificationCodes = [];
    public virtual IReadOnlyCollection<VerificationCode> VerificationCodes => _verificationCodes;
    public virtual ICollection<UserClaim> UserClaims { get; private set; } = new List<UserClaim>();

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
    /// <param name="codeType"><see cref="VerificationCodeType"/></param>
    public void AddVerificationCode(VerificationCodeType codeType)
    {
        var existingCodes = VerificationCodes.Where(vc => vc.CodeType == codeType && !vc.IsUsed);

        foreach (var code in existingCodes)
        {
            code.Use();
        }

        _verificationCodes.Add(new(Guid.NewGuid(), userId: Id, codeType));
    }

    /// <summary>
    /// Returns the most recent active verification code of the specified type.
    /// <para>
    /// Returns null if no active codes are found.
    /// </para>
    /// </summary>
    /// <param name="codeType"><see cref="VerificationCodeType"/></param>
    /// <returns></returns>
    public VerificationCode? GetActiveVerificationCode(VerificationCodeType codeType)
    {
        return VerificationCodes.Where(vc => vc.CodeType == codeType && !vc.IsUsed && vc.ExpirationDate > DateTime.UtcNow).MaxBy(vc => vc.ExpirationDate);
    }

    /// <summary>
    /// Checks the count of the verification codes and verifies the user if the code is valid.
    /// </summary>
    /// <param name="verificationCode">Value representing the VerificationCode.Code <see cref="VerificationCode.Code"/></param>
    /// <returns>Result representing the verification result</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Result Verify(string verificationCode)
    {
        if (VerificationCodes.Count == 0)
            throw new InvalidOperationException("This chat user does not have a verification code");

        if (IsVerified)
            throw new InvalidOperationException("This chat user is already verified");

        var code = VerificationCodes.FirstOrDefault(c => c.Code == verificationCode);
        if (code is null) return Result.Fail(Errors.General.UnspecifiedError("Invalid verification code"));

        var verificationResult = code.Verify(verificationCode);

        if (verificationResult.Success is false)
            return verificationResult;

        code.Use();
        IsVerified = true;
        AddDomainEvent(new UserVerifiedEvent(Id));
        return Result.Ok();
    }
}