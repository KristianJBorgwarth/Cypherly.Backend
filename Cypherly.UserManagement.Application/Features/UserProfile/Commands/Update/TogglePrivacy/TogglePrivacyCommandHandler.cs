using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    ILogger<TogglePrivacyCommandHandler> logger)
    : ICommandHandler<TogglePrivacyCommand>
{
    public async Task<Result> Handle(TogglePrivacyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogCritical("User with {ID} not found", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            userProfile.TogglePrivacy(request.IsPrivate);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occurred while toggling privacy for user with {ID}", request.Id);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occurred while attempting to toggle privacy"));
        }
    }
}