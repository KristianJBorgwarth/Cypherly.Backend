using Cypherly.Application.Abstractions;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public sealed record DisconnectCommand : ICommand
{
    public required string TransientConnectionId { get; init; }
}