using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandValidator : AbstractValidator<TogglePrivacyCommand>
{
    public TogglePrivacyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(TogglePrivacyCommand.Id)).Message);
    }
}