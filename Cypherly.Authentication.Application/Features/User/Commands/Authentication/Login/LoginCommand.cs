using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;

public sealed record LoginCommand : ICommand<LoginDto>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}