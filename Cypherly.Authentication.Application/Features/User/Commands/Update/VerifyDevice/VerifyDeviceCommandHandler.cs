using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;

public class VerifyDeviceCommandHandler(
    IUserRepository userRepository,
    IDeviceService deviceService,
    IUnitOfWork unitOfWork,
    ILogger<VerifyDeviceCommandValidator> logger)
    : ICommandHandler<VerifyDeviceCommand>
{
    public async Task<Result> Handle(VerifyDeviceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogCritical("Error occurred while verifying device. User with ID {UserId} not found.", request.UserId);
                return Result.Fail(Errors.General.NotFound(request.UserId));
            }

            var result = deviceService.VerifyDevice(user, request.DeviceId, request.DeviceVerificationCode);

            if (result.Success is false) return result;

            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while verifying device.");
            return Result.Fail(Errors.General.UnspecifiedError("An Exception occurred while verifying device."));
        }
    }
}