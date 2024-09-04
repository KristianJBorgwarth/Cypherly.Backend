using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;

public class VerifyUserCommandHandler(
    ILogger<VerifyUserCommandHandler> logger,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<VerifyUserCommand>
{
    public async Task<Result> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Fail(Errors.General.NotFound(request.UserId));

            var result = user.Verify(request.VerificationCode);
            if (result.Success is false) return Result.Fail(result.Error);

            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling VerifyUserCommand");
            return Result.Fail(Errors.General.UnspecifiedError("Exception occured attempting to verify the user. Check logs for more information"));
        }
    }
}