using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public sealed class ConnectCommandHandler : ICommandHandler<ConnectCommand>
{
    public async Task<Result> Handle(ConnectCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}