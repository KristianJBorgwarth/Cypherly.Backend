using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;

public class DeleteFriendshipCommandValidator : AbstractValidator<DeleteFriendshipCommand>
{
    public DeleteFriendshipCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x=> x.UserProfileId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(DeleteFriendshipCommand.UserProfileId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendshipCommand.UserProfileId)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(DeleteFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendshipCommand.FriendTag)).Message)
            .Must(x => x.Length <= 20)
            .WithMessage(Errors.General.ValueTooLarge(nameof(DeleteFriendshipCommand.FriendTag), 20).Message);
        
    }
}