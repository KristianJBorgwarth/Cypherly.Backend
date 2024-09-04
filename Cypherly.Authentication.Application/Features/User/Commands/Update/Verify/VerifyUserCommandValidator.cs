using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;

public class VerifyUserCommandValidator : AbstractValidator<VerifyUserCommand>
{
    public VerifyUserCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(VerifyUserCommand.UserId)).Message);

        RuleFor(cmd => cmd.VerificationCode)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(VerifyUserCommand.VerificationCode)).Message);
    }
}