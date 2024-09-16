using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandValidator : AbstractValidator<UpdateUserProfileDisplayNameCommand>
{
    public UpdateUserProfileDisplayNameCommandValidator()
    {
        RuleFor(cmd => cmd.UserProfileId)
            .NotNull().NotEmpty().WithMessage(Errors.General
                .ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.UserProfileId)).Message);
        RuleFor(cmd => cmd.DisplayName)
            .NotNull().NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.DisplayName)).Message);
    }
}