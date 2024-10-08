using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public class UserBlockedEventHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork uow,
    ILogger<UserBlockedEventHandler> logger)
    : IDomainEventHandler<UserBlockedEvent>
{
    public async Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken)
    {
        var blockedUserProfile = await userProfileRepository.GetByIdAsync(notification.BlockedUserProfileId);
        var userProfile = await userProfileRepository.GetByIdAsync(notification.UserProfileId);

        if (blockedUserProfile is null)
        {
            logger.LogError("Blocked userprofile ID not found {0}", notification.BlockedUserProfileId);
            return;
        }

        if (userProfile is null)
        {
            logger.LogError("Userprofile ID not found {0}", notification.UserProfileId);
            return;
        }

        userProfile.DeleteFriendship(blockedUserProfile.UserTag.Tag);
        blockedUserProfile.DeleteFriendship(blockedUserProfile.UserTag.Tag);

        await uow.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {0} blocked user {1}", userProfile.UserTag.Tag, blockedUserProfile.UserTag.Tag);
    }
}