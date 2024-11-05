namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create;

public sealed record CreateClaimDto
{
    public Guid Id { get; init; }
    public required string ClaimType { get; init; }
}