using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public sealed record CreateUserCommand : ICommand<CreateUserDto>
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}