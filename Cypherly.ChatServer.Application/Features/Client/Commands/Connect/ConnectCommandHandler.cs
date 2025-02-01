using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public sealed class ConnectCommandHandler(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IClientCache clientCache,
    ILogger<ConnectCommandHandler> logger)
    : ICommandHandler<ConnectCommand>
{
    public async Task<Result> Handle(ConnectCommand command, CancellationToken cancellationToken)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.LogCritical("Exception occurred in ConnectCommandHandler: {Exception}. For Client with ID: {ID}", e, command.ClientId);
            return Result.Fail(Errors.General.UnspecifiedError("ConnectCommandHandler"));
        }
    }
}