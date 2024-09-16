namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;

public sealed record UpdateUserProfileDisplayNameDto
{
    public required string DisplayName { get; init; }
}