using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Device.Commands.Create;

public class CreateDeviceCommandHandler(
    IUserRepository userRepository,
    ILoginNonceCache loginNonceCache,
    IDeviceService deviceService,
    IUnitOfWork unitOfWork,
    ILogger<CreateDeviceCommandHandler> logger)
    : ICommandHandler<CreateDeviceCommand, CreateDeviceDto>
{
    public async Task<Result<CreateDeviceDto>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("User {UserId} not found", request.UserId);
                return Result.Fail<CreateDeviceDto>(Errors.General.NotFound(request.UserId));
            }

            var loginNonce = await loginNonceCache.GetNonceAsync(request.LoginNonceId, cancellationToken);

            if (loginNonce is null || loginNonce.NonceValue != request.LoginNonce)
            {
                logger.LogWarning("Login nonce {LoginNonceId} not found or invalid for user with ID: {ID}", request.LoginNonceId, request.UserId);
                return Result.Fail<CreateDeviceDto>(Errors.General.Unauthorized());
            }

            var device = deviceService.RegisterDevice(user, request.Base64DevicePublicKey, request.DeviceAppVersion, request.DeviceType, request.DevicePlatform);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = CreateDeviceDto.Map(device);

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "exception occured while creating device for user {UserId}", request.UserId);
            return Result.Fail<CreateDeviceDto>(Errors.General.UnspecifiedError("An exception occured while creating device"));
        }
    }
}