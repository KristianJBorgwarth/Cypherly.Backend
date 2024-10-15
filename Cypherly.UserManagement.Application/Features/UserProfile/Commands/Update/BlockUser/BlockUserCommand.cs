using System.Windows.Input;
using ICommand = Cypherly.Application.Abstractions.ICommand;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommand
{
    public required Guid Id { get; init; } 
    public required string BlockedUserTag { get; init; }
}