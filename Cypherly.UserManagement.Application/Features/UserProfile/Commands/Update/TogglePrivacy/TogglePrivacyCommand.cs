using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public sealed record TogglePrivacyCommand : ICommandId
{
    public Guid Id { get; init; } // The ID of the user whose privacy status is being toggled
    public bool IsPrivate { get; init; }
}