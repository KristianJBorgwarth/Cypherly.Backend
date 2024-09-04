using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateUserCommand.Email)).Message);
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateUserCommand.Password)).Message);
    }
}