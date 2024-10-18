using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryValidator : AbstractValidator<GetUserProfileByTagQuery>
{
    public GetUserProfileByTagQueryValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserProfileByTagQuery.Id)).Message);
        RuleFor(x => x.Tag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetUserProfileByTagQuery.Tag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserProfileByTagQuery.Tag)).Message);
    }
}