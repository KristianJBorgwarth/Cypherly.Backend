using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;

public class GetUserDevicesQueryHandler(
    IUserRepository userRepository,
    ILogger<GetUserDevicesQueryHandler> logger)
    : IQueryHandler<GetUserDevicesQuery, GetUserDevicesDto>
{
    public async Task<Result<GetUserDevicesDto>> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve devices for the specified user
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("User with ID: {ID} not found.", request.UserId);
                return Result.Fail<GetUserDevicesDto>(Errors.General.NotFound(request.UserId));
            }
            var devices = user.GetValidDevices();
            
            return Result.Ok(GetUserDevicesDto.Map(devices));

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred in {Handler}, while attempting to retrieve devices for {UserId}",
                nameof(GetUserDevicesQueryHandler), request.UserId);
            return Result.Fail<GetUserDevicesDto>(Errors.General.UnspecifiedError("An exception occurred while attempting to retrieve devices."));
        }
    }
}