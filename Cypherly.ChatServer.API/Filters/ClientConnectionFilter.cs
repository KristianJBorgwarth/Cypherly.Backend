using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Filters;

public class ClientConnectionFilter(ISender sender) : IHubFilter
{
    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next, CancellationToken cancellationToken)
    {
        var httpContext = context.Context.GetHttpContext();
        var deviceId = httpContext?.User.FindFirst("sub")?.Value;

        if (deviceId != null && Guid.TryParse(deviceId, out var deviceIdGuid))
        {
            var result = await sender.Send(new ConnectCommand() { ClientId = deviceIdGuid, TransientId = context.Context.ConnectionId }, cancellationToken);

            if (result.Success is false)
            {
                context.Context.Abort();
                return;
            }
        }
        else
        {
            context.Context.Abort();
            return;
        }

        await next(context);
    }

    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        await sender.Send(new DisconnectCommand() { TransientId = context.Context.ConnectionId });
        await next(context, exception);
    }
}