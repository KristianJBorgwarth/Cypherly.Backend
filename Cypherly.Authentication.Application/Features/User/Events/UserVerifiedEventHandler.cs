using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Events;

public sealed class UserVerifiedEventHandler(
    IUserRepository userRepository,
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserVerifiedEventHandler> logger)
    : IDomainEventHandler<UserVerifiedEvent>
{
    public async Task Handle(UserVerifiedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(notification.UserId);
            if (user is null)
            {
                logger.LogError("User with ID {UserId} not found during {UserVerifiedEventHandler}",
                    notification.UserId, nameof(UserVerifiedEventHandler));
                return;
            }

            var claim = await claimRepository.GetClaimByTypeAsync("user", cancellationToken);
            if (claim is null)
            {
                logger.LogError("Claim with type 'user' not found during {UserVerifiedEventHandler}",
                    nameof(UserVerifiedEventHandler));
                return;
            }

            claim.AddUserClaim(new(Guid.NewGuid(), user.Id, claim.Id));
            await claimRepository.UpdateAsync(claim);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User with ID {UserId} verified during {UserVerifiedEventHandler}",
                notification.UserId, nameof(UserVerifiedEventHandler));
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error handling {UserVerifiedEventHandler} for UserProfile with Id {id}", nameof(UserVerifiedEventHandler), notification.UserId);
        }
    }
}