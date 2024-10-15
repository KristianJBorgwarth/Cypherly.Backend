using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Events;

public class UserBlockedEventHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<UserBlockedEventHandler> logger)
    : IDomainEventHandler<UserBlockedEvent>
{
    public async Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken)
    {
        //TODO - Implement logic to notify blocked user to delete their friendship data, if we decide to cache friendship data in the future
    }
}