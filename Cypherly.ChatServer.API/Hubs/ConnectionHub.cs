using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using Cypherly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

public sealed class ConnectionHub(ISender sender) : Hub
{
    public async Task<Result> ConnectAsync(ConnectCommand cmd)
    {
        var result = await sender.Send(cmd);
        return result;
    }

    public override async Task OnConnectedAsync()
    {
        var transientId = Context.ConnectionId;
        var result = await sender.Send(new DisconnectCommand() {TransientConnectionId = transientId});
    }
}