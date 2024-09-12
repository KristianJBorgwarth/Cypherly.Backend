using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;

public class GetUserProfileByIdQueryValidator : AbstractValidator<GetUserProfileByIdQuery>
{
    public GetUserProfileByIdQueryValidator()
    {
        RuleFor(x=> x.UserProfileId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetUserProfileByIdQuery.UserProfileId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserProfileByIdQuery.UserProfileId)).Message);
    }
}