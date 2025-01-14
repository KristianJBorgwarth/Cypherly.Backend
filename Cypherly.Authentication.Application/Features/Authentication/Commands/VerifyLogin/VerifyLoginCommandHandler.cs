using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Caching.LoginNonce;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyLogin;

public sealed class VerifyLoginCommandHandler(
    IUserRepository userRepository,
    ILoginNonceCache loginNonceCache,
    IUnitOfWork unitOfWork,
    ILogger<VerifyLoginCommandHandler> logger)
    : ICommandHandler<VerifyLoginCommand, VerifyLoginDto>
{
    public async Task<Result<VerifyLoginDto>> Handle(VerifyLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                return Result.Fail<VerifyLoginDto>(Errors.General.NotFound(request.UserId));
            }

            var verificationResult = user.VerifyLogin(request.LoginVerificationCode);
            if(verificationResult.Success is false)
            {
                return Result.Fail<VerifyLoginDto>(verificationResult.Error);
            }

            var loginNonce = LoginNonce.Create(request.UserId);

            await loginNonceCache.AddNonceAsync(loginNonce, cancellationToken);

            var dto = VerifyLoginDto.Map(loginNonce);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error verifying login for user {UserId}", request.UserId);
            return Result.Fail<VerifyLoginDto>(Errors.General.UnspecifiedError("An exception occurred while verifying login"));
        }
    }
}