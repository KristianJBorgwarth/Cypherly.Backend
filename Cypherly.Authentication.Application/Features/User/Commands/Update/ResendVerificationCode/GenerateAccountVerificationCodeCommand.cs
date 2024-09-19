using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public sealed record GenerateAccountVerificationCodeCommand : ICommand
{
    public required Guid UserId { get; init; }
}