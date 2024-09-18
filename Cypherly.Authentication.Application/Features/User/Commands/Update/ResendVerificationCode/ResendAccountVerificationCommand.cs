using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public sealed record ResendAccountVerificationCommand : ICommand
{
    public required Guid UserId { get; init; }
}