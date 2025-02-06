using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public class DisconnectClientCommandHandler(
    IClientRepository clientRepository,
    IClientCache clientCache,
    IUnitOfWork unitOfWork,
    ILogger<DisconnectClientCommandHandler> logger)
    : ICommandHandler<DisconnectClientCommand>
{
    public async Task<Result> Handle(DisconnectClientCommand command, CancellationToken cancellationToken)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.LogCritical("Exception occurred in DisconnectClientCommandHandler: {Exception}. For Client with Transient ID: {ID}", e, command.TransientId);
            return Result.Fail(Errors.General.UnspecifiedError("DisconnectClientCommandHandler"));
        }
    }
}