using Cypherly.ChatServer.Application.Contracts;
using Cypherly.Common.Messaging.Messages.PublishMessages.UserProfile;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Features.ChangeEvent.ProfilePictureUpdated;

public class ProfilePictureUpdatedConsumer(
    IChangeEventNotifier changeEventNotifier,
    IClientCache clientCache,
    ILogger<ProfilePictureUpdatedConsumer> logger)
    : IConsumer<ProfilePictureUpdatedMessage>
{
    public async Task Consume(ConsumeContext<ProfilePictureUpdatedMessage> context)
    {
        try
        {
            var message = context.Message;
            var eventData = new ProfilePictureUpdatedChangeEvent(message.UserProfileId, message.ProfilePictureUrl);
            var changeEvent = new ChangeEvent(Guid.NewGuid(), ChangeEventType.ProfilePictureChanged, "UserProfile", "Profile picture updated", eventData);

            foreach (var connectionId in message.ConnectionIds)
            {
                var client = await clientCache.GetAsync(connectionId, context.CancellationToken);
                if (client == null)
                {
                    logger.LogWarning("Client not found for connectionId {ConnectionId}", connectionId);
                    continue;
                }
                await changeEventNotifier.NotifyAsync(connectionId.ToString(), changeEvent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing ProfilePictureUpdatedMessage");
        }
    }
}