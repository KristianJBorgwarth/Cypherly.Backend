using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandValidator : AbstractValidator<CreateFriendshipCommand>
{
    public CreateFriendshipCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.Id)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(CreateFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.FriendTag)).Message);
    }
}