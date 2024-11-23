﻿using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Authentication.Queries.GetNonce;

public class GetNonceQueryValidator : AbstractValidator<GetNonceQuery>
{
    public GetNonceQueryValidator()
    {
        RuleFor(x=> x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetNonceQuery.UserId)).Message);

        RuleFor(x=> x.DeviceId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(GetNonceQuery.DeviceId)).Message);
    }
}