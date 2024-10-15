﻿using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;

public sealed record UpdateUserProfileDisplayNameCommand : ICommand<UpdateUserProfileDisplayNameDto>
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}