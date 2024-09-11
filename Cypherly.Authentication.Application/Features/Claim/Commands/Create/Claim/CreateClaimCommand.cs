﻿using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;

public sealed record CreateClaimCommand : ICommand<CreateClaimDto>
{
    public required string ClaimType { get; init; }
}