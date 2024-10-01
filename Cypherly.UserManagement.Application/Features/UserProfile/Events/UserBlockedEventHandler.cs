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

    public Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}