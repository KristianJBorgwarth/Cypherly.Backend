using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Filters;

public class ClientConnectionFilter(
    ISender sender,
    ILogger<ClientConnectionFilter> logger)
    : IHubFilter
{
    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Client connected: {ClientId}", context.Context.ConnectionId);
        var httpContext = context.Context.GetHttpContext();
        var deviceId = httpContext?.User.FindFirst("sub")?.Value;

        if (deviceId != null && Guid.TryParse(deviceId, out var deviceIdGuid))
        {
            var result = await sender.Send(new ConnectClientCommand() { ClientId = deviceIdGuid, TransientId = context.Context.ConnectionId }, cancellationToken);

            if (result.Success is false)
            {
                logger.LogWarning("ConnectClientCommand failed for client: {ClientId}", deviceIdGuid);
                context.Context.Abort();
                return;
            }
            logger.LogInformation("Client connected: {ClientId}", deviceIdGuid);
        }
        else
        {
            logger.LogWarning("Client connection failed: {ClientId}", deviceId);
            context.Context.Abort();
            return;
        }

        await next(context);
    }

    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        logger.LogInformation("Client disconnected: {ClientId}", context.Context.ConnectionId);
        await sender.Send(new DisconnectClientCommand() { TransientId = context.Context.ConnectionId });
        await next(context, exception);
    }
}