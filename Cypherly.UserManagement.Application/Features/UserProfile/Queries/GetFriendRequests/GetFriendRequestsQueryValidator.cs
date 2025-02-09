using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsQueryValidator : AbstractValidator<GetFriendRequestsQuery>
{
    public GetFriendRequestsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(GetFriendRequestsQuery.UserId)).Message);
    }
}