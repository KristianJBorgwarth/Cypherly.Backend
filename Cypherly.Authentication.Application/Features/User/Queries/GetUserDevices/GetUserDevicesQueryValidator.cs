using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;

public class GetUserDevicesQueryValidator : AbstractValidator<GetUserDevicesQuery>
{
    public GetUserDevicesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetUserDevicesQuery.UserId)).Message);
    }
}