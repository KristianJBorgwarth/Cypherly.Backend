using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public sealed record ResendVerificationCodeCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required UserVerificationCodeType CodeType { get; init; }
}