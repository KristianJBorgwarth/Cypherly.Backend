using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Caching;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Queries.GetNonce;

public class GetNonceQueryHandler(
    IUserRepository userRepository,
    INonceCacheService nonceCache,
    ILogger<GetNonceQueryHandler> logger)
    : IQueryHandler<GetNonceQuery, GetNonceDto>
{
    public async Task<Result<GetNonceDto>> Handle(GetNonceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("User with ID: {ID} not found.", request.UserId);
                return Result.Fail<GetNonceDto>(Errors.General.NotFound("User not found."));
            }

            var device = user.GetDevice(request.DeviceId);

            var nonce = Nonce.Create(user.Id, device.Id);

            await nonceCache.AddNonceAsync(nonce, cancellationToken);

            var dto = GetNonceDto.Map(nonce);

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Exception occurred attempting to fetch nonce for user with ID: {UserId} and Device ID: {DeviceId}.", request.UserId, request.DeviceId);
            return Result.Fail<GetNonceDto>(Errors.General.UnspecifiedError("An exception occured attempting to fetch nonce."));
        }
    }
}