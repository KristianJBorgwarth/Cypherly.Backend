using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;

public class VerifyDeviceCommandHandler(
    IUserRepository userRepository,
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

            var device = user.GetDevice(request.DeviceId);

            device.Verify(request.DeviceVerificationCode);

            await userRepository.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}