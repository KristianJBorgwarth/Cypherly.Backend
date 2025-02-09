using Cypherly.ChatServer.Application.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.ChangeEvent.ProfilePictureUpdated;

public class ProfilePictureUpdatedConsumer(
    IChangeEventNotifier changeEventNotifier,
    IClientCache clientCache,
    ILogger<ProfilePictureUpdatedConsumer> logger)
    : IConsumer<ProfilePictureUpdatedConsumer>
{
    public Task Consume(ConsumeContext<ProfilePictureUpdatedConsumer> context)
    {
        throw new NotImplementedException();
    }
}