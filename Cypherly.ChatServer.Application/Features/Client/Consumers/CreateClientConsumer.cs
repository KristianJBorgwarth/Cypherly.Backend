using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.Common.Messaging.Messages.RequestMessages.Client;
using Cypherly.Domain.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.Client.Consumers;

public class CreateClientConsumer(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateClientConsumer> logger)
    : IConsumer<CreateClientRequest>
{
    public async Task Consume(ConsumeContext<CreateClientRequest> context)
    {
        try
        {
            var message = context.Message;

            var client = new Domain.Aggregates.Client(message.DeviceId, message.ConnectionId);

            await clientRepository.CreateAsync(client);

            await unitOfWork.SaveChangesAsync();

            await context.RespondAsync(new CreateClientResponse(isSuccess: true, error: null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while attempting to create a client");
            await context.RespondAsync(new CreateClientResponse(false, Errors.General.UnspecifiedError("An Exception occured while attempting to create a client").Message));
        }
    }
}