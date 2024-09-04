namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public sealed record CreateUserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
}