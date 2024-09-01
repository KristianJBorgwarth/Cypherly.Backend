namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public sealed class CreateUserDto
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}