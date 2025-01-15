using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Device.Queries.GetDevices;

public class GetDevicesQueryValidator : AbstractValidator<GetDevicesQuery>
{
    public GetDevicesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(GetDevicesQuery.UserId)).Message);
    }
}