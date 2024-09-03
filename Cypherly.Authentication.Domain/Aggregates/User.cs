using System.Diagnostics.CodeAnalysis;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Aggregates;

[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class User : AggregateRoot
{
    public Email Email { get; init; } = null!;
    public Password Password { get; private set; } = null!;
    public bool IsVerified { get; private set; }
    public virtual VerificationCode? VerificationCode { get; private set; }

    public User() {} // For EF Core

    public User(Email email, Password password, bool isVerified)
    {
        Email = email;
        Password = password;
        IsVerified = isVerified;
    }

    public void SetVerificationCode() => VerificationCode = new(Id);

    public Result Verify(string verificationCode)
    {
        if(VerificationCode is null)
            return Result.Fail(Errors.General.UnspecifiedError("This chat user does not have a verification code"));

        if(IsVerified)
            return Result.Fail(Errors.General.UnspecifiedError("This chat user is already verified"));

        var verificationResult = VerificationCode.Verify(verificationCode);

        if (verificationResult.Success is false)
            return verificationResult;

        VerificationCode.Use();
        IsVerified = true;
        return Result.Ok();
    }
}