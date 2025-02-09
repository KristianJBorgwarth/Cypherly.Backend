using Cypherly.ChatServer.Application.Features.ChangeEvent;

namespace Cypherly.ChatServer.Application.Contracts;

public interface IChangeEventNotifier
{
    Task NotifyAsync(string ConnectionId, ChangeEvent changeEvent, CancellationToken cancellationToken = default);
}