﻿using Cypherly.Application.Abstractions;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public class DisconnectClientCommandHandler(
    IClientCache clientCache,
    ILogger<DisconnectClientCommandHandler> logger)
    : ICommandHandler<DisconnectClientCommand>
{
    public async Task<Result> Handle(DisconnectClientCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var client = await clientCache.GetByTransientIdAsync(command.TransientId, cancellationToken);

            if (client is null)
            {
                logger.LogWarning("Client with Transient ID: {ID} not found in cache", command.TransientId);
                return Result.Fail(Errors.General.NotFound("Client"));
            }

            await clientCache.RemoveAsync(client.ConnectionId, cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogCritical("Exception occurred in DisconnectClientCommandHandler: {Exception}. For Client with Transient ID: {ID}", e, command.TransientId);
            return Result.Fail(Errors.General.UnspecifiedError("DisconnectClientCommandHandler"));
        }
    }
}