using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public class ResendAccountVerificationCommandValidator : AbstractValidator<ResendAccountVerificationCommand>
{
    public ResendAccountVerificationCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(ResendAccountVerificationCommand.UserId)).Message);
    }
}