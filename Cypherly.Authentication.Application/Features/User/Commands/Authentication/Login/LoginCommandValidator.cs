using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.Email)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.Email)).Message)
            .Must(email => email.Length < 256).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.Email), 255).Message);

        RuleFor(x => x.Password)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.Password)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.Password)).Message)
            .Must(pw=> pw.Length < 256).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.Password), 255).Message);
    }
}