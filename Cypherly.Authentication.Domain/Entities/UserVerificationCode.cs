using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class UserVerificationCode : Entity
{
    public Guid UserId { get; private set; }
    public VerificationCode Code { get; private set; } = null!;
    public UserVerificationCodeType CodeType { get; private set; }
    public UserVerificationCode() : base(Guid.Empty){ } // For EF Core

    public UserVerificationCode(Guid id, Guid userId, UserVerificationCodeType codeType) : base(id)
    {
        UserId = userId;
        CodeType = codeType;
        Code = GenerateVerificationCode(codeType);
    }

    private static VerificationCode GenerateVerificationCode(UserVerificationCodeType type)
    {
        return type switch
        {
            UserVerificationCodeType.EmailVerification => VerificationCode.Create(DateTime.UtcNow.AddHours(1)),
            UserVerificationCodeType.PasswordReset => VerificationCode.Create(DateTime.UtcNow.AddMinutes(15)),
            UserVerificationCodeType.Login => VerificationCode.Create(DateTime.UtcNow.AddMinutes(10)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}