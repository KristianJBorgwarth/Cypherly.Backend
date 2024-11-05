using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create;

public sealed record CreateClaimCommand : ICommand<CreateClaimDto>
{
    public required string ClaimType { get; init; }
}