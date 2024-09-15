using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(UpdateUserProfilePictureCommand.Id))
                .Message)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.Id)).Message);

        RuleFor(cmd => cmd.NewProfilePicture)
            .NotNull().WithMessage(Errors.General
                .ValueIsRequired(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message)
            .NotEmpty().WithMessage(Errors.General
                .ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message);
    }
}