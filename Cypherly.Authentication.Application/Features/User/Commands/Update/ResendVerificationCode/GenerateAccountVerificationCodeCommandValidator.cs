using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public class GenerateAccountVerificationCodeCommandValidator : AbstractValidator<GenerateAccountVerificationCodeCommand>
{
    public GenerateAccountVerificationCodeCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GenerateAccountVerificationCodeCommand.UserId)).Message);
    }
}