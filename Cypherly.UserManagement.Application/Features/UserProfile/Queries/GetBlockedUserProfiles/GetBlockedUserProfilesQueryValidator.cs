using Cypherly.Domain.Common;
using Cypherly.Domain.ValueObjects;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryValidator : AbstractValidator<GetBlockedUserProfilesQuery>
{
    public GetBlockedUserProfilesQueryValidator()
    {
        RuleFor(x=> x.UserId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(GetBlockedUserProfilesQuery.UserId)).Message);
    }
}