using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public class DisconnectClientCommandHandler : ICommandHandler<DisconnectClientCommand>
{
    public async Task<Result> Handle(DisconnectClientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}