using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public sealed class ConnectionHub(ISender sender) : Hub
{
    public async Task ConnectAsync(ConnectCommand cmd)
    {
        var result = await sender.Send(cmd);
    }

    public override async Task OnConnectedAsync()
    {
        var transientId = Context.ConnectionId;
        var result = await sender.Send(new DisconnectCommand() {TransientConnectionId = transientId});
    }
}