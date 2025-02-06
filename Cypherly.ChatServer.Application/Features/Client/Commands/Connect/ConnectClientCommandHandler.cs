using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Cache.Client;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Domain.Events.Client;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public sealed class ConnectClientCommandHandler(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IClientCache clientCache,
    ILogger<ConnectClientCommandHandler> logger)
    : ICommandHandler<ConnectClientCommand>
{
    public async Task<Result> Handle(ConnectClientCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var client = await clientRepository.GetByIdAsync(command.ClientId);

            if (client is null)
            {
                logger.LogError("Client with ID: {ID} not found in database. During ConnectClientCommand operation", command.ClientId);
                return Result.Fail(Errors.General.Unauthorized());
            }

            var cachableClient = ClientCacheDto.Create(client, command.TransientId);

            await clientCache.AddAsync(cachableClient, cancellationToken);

            client.AddDomainEvent(new ClientConnectedEvent(client.Id));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogCritical("Exception occurred in ConnectCommandHandler: {Exception}. For Client with ID: {ID}", e, command.ClientId);
            return Result.Fail(Errors.General.UnspecifiedError("ConnectCommandHandler"));
        }
    }
}