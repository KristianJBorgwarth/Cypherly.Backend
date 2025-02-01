using Cypherly.Application.Abstractions;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public sealed record ConnectCommand : ICommand
{
    public required Guid CliendId { get; init; }
}