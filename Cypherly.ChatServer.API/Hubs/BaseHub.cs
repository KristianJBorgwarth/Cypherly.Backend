using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public class BaseHub(
    ISender sender,
    ILogger<BaseHub> logger)
    : Hub
{
    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var deviceId = httpContext?.User.FindFirst("sub")?.Value;

        logger.LogInformation("OnConnectedAsync triggered for connection: {ConnectionId}", Context.ConnectionId);


        if (deviceId != null && Guid.TryParse(deviceId, out var deviceIdGuid))
        {
            var result = await sender.Send(new ConnectClientCommand {
                ClientId = deviceIdGuid,
                TransientId = Context.ConnectionId,
            });

            if (result.Success is false)
            {
                logger.LogWarning("ConnectClientCommand failed for client: {ClientId}", deviceIdGuid);
                Context.Abort();
                return;
            }
            logger.LogInformation("Client connected: {ClientId}, TransientId: {TransientId}",
                deviceIdGuid, Context.ConnectionId);
        }
        else
        {
            logger.LogWarning("Client connection failed: {ClientId}", deviceId);
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("OnDisconnectedAsync triggered for connection: {ConnectionId}", Context.ConnectionId);
        await sender.Send(new DisconnectClientCommand { TransientId = Context.ConnectionId });
        await base.OnDisconnectedAsync(exception);
    }
}