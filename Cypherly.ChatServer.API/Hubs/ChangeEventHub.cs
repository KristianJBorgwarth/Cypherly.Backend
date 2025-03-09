using Cypherly.ChatServer.Application.Features.ChangeEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cypherly.ChatServer.API.Hubs;

[Authorize]
public class ChangeEventHub(
    ISender sender,
    ILogger<BaseHub> logger)
    : BaseHub(sender, logger)
{
    public async Task SendChangeNotification(string connectionId, ChangeEvent changeEvent)
    {
        await Clients.Client(connectionId).SendAsync("ChangeNotification", changeEvent);
    }
}