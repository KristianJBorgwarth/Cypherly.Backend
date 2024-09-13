using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update;

public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
    }
}