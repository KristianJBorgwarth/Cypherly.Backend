using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;

public sealed record UnblockUserCommand : ICommandId
{
    public Guid Id { get; init; }
    public required string Tag { get; init; }
}