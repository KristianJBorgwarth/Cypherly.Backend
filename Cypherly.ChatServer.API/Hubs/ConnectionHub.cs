using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using Cypherly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public sealed class ConnectionHub(ISender sender) : Hub
{
    public async Task ConnectAsync(ConnectCommand cmd)
    {
        var result = await sender.Send(cmd);
        if (result.Success is false)
        {
            throw new HubException("Failed to map connection to client.");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await sender.Send(new DisconnectCommand() { TransientConnectionId = Context.ConnectionId });
        await base.OnDisconnectedAsync(exception);
    }
}