using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand.UserId)).Message);

        RuleFor(x => x.BlockedUserTag)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand)).Message);
    }
}