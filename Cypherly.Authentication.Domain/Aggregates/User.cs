using System.Diagnostics.CodeAnalysis;
using Cypherly.Authentication.Domain.Entities;
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

    public User() : base(Guid.Empty)
    {
    } // For EF Core

    public User(Guid id, Email email, Password password, bool isVerified) : base(id)
    {
        Email = email;
        Password = password;
        IsVerified = isVerified;
    }

    public void AddVerificationCode()
    {
        _verificationCodes.Add(new(Guid.NewGuid(), userId: Id));
    }

    public VerificationCode? GetVerificationCode()
    {
        if (VerificationCodes.Count == 0)
            throw new InvalidOperationException("This chat user does not have a verification code");

        var code = VerificationCodes.Where(vc => vc.ExpirationDate > DateTime.UtcNow && !vc.IsUsed)
            .MaxBy(vc => vc.ExpirationDate);

        return code;
    }

    public Result Verify(string verificationCode)
    {
        if (VerificationCodes.Count == 0)
            throw new InvalidOperationException("This chat user does not have a verification code");

        if (IsVerified)
            throw new InvalidOperationException("This chat user is already verified");

        var code = VerificationCodes.FirstOrDefault(c => c.Code == verificationCode);
        if (code is null)
            return Result.Fail(Errors.General.UnspecifiedError("Invalid verification code"));

        var verificationResult = code.Verify(verificationCode);

        if (verificationResult.Success is false)
            return verificationResult;

        code.Use();
        IsVerified = true;
        AddDomainEvent(new UserVerifiedEvent(Id));
        return Result.Ok();
    }
}