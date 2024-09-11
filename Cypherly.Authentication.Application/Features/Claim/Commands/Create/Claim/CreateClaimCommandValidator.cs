using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;

public class CreateUserClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateUserClaimCommandValidator()
    {

    }
}