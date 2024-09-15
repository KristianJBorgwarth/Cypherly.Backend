using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Domain.Events.UserProfile;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public class UserProfilePictureUpdatedEventHandler : IDomainEventHandler<UserProfilePictureUpdatedEvent>
{
    //TODO: Implement some notification logic to notify friends of the user with the updated profile picture
    public Task Handle(UserProfilePictureUpdatedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}