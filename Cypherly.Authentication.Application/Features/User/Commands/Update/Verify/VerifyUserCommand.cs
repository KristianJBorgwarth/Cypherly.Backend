using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;

public sealed record VerifyUserCommand : ICommand<Result>
{
    public required Guid UserId { get; init; }
    public required string VerificationCode { get; init; }
}