using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public sealed class ResendVerificationCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<ResendVerificationCommandHandler> logger)
    : ICommandHandler<ResendVerificationCommand>
{
    public async Task<Result> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("User {UserId} not found", request.UserId);
                return Result.Fail(Errors.General.NotFound(request.UserId));
            }

            if(user.IsVerified is true)
            {
                logger.LogWarning("User {UserId} is already verified", request.UserId);
                return Result.Fail(Errors.General.UnspecifiedError("User is already verified"));
            }

            //TODO: refactor verification code relationship to list, so we dont have to manage single code
            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred while resending verification code for user {UserId}", request.UserId);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occurred while resending verification code for user"));
        }
    }
}