using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;

public sealed class GenerateAccountVerificationCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IVerificationCodeService verificationCodeService,
    ILogger<GenerateAccountVerificationCodeCommandHandler> logger)
    : ICommandHandler<GenerateAccountVerificationCodeCommand>
{
    public async Task<Result> Handle(GenerateAccountVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("User {UserId} not found", request.UserId);
                return Result.Fail(Errors.General.NotFound(request.UserId));
            }

            if(user.IsVerified)
            {
                logger.LogWarning("User {UserId} is already verified", request.UserId);
                return Result.Fail(Errors.General.UnspecifiedError("User is already verified"));
            }

            verificationCodeService.GenerateVerificationCode(user, VerificationCodeType.EmailVerification);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred while resending verification code for user {UserId}", request.UserId);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occurred while resending verification code for user"));
        }
    }
}