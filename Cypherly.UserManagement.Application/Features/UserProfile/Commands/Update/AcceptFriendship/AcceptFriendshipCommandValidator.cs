using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandValidator : AbstractValidator<AcceptFriendshipCommand>
{
    public AcceptFriendshipCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.Id)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(AcceptFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.FriendTag)).Message);
    }
}