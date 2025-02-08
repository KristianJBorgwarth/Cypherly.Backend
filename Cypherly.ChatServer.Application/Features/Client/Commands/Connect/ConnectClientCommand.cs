using Cypherly.Application.Abstractions;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public sealed record ConnectClientCommand : ICommand
{
    public required Guid ClientId { get; init; }
    public required string TransientId { get; init; }
}