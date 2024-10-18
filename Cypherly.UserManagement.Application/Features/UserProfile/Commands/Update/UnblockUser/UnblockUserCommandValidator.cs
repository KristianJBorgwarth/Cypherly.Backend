using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;

public class UnblockUserCommandValidator :  AbstractValidator<UnblockUserCommand>
{
    public UnblockUserCommandValidator()
    {
        
    }
}