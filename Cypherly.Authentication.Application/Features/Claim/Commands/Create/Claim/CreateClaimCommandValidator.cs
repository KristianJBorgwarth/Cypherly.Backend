using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(v => v.ClaimType)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateClaimCommand.ClaimType)).Message);
    }
}