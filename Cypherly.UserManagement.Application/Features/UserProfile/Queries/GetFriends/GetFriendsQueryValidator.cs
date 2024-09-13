using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;

public class GetFriendsQueryValidator : AbstractValidator<GetFriendsQuery>
{
    public GetFriendsQueryValidator()
    {
        RuleFor(x=> x.UserProfileId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendsQuery.UserProfileId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.UserProfileId)).Message);
    }
}