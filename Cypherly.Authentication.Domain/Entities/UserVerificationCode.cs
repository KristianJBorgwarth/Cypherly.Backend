﻿using Cypherly.Authentication.Domain.Enums;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class UserVerificationCode : Entity
{
    public Guid UserId { get; private set; }
    public string Code { get; } = null!;
    public UserVerificationCodeType CodeType { get; private set; }
    public DateTime ExpirationDate { get; }
    public bool IsUsed { get; private set; }

    public UserVerificationCode() : base(Guid.Empty){ } // For EF Core

    public UserVerificationCode(Guid id, Guid userId, UserVerificationCodeType codeType) : base(id)
    {
        UserId = userId;
        CodeType = codeType;
        Code = GenerateCode();
        //TODO: set the expiration date based on the code type (e.g. email verification code expiration)
        ExpirationDate = DateTime.UtcNow.AddHours(1);
        IsUsed = false;
    }

    /// <summary>
    /// Validates and verifies the verification code
    /// </summary>
    /// <param name="code">The code being verified</param>
    /// <returns>Boolean value representing the verification</returns>
    public Result Verify(string code)
    {
        if (IsUsed)
            return Result.Fail(Errors.General.UnspecifiedError("Verification code has already been used"));
        if(DateTime.UtcNow > ExpirationDate)
            return Result.Fail(Errors.General.UnspecifiedError("Verification code has expired"));
        if (Code != code)
            return Result.Fail(Errors.General.UnspecifiedError("Invalid verification code"));

        return Result.Ok();
    }

    /// <summary>
    /// Marks the verification code as used
    /// </summary>
    public void Use()
    {
        IsUsed = true;
    }

    /// <summary>
    /// Generates a random 6-digit verification code
    /// </summary>
    /// <returns></returns>
    private static string GenerateCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}