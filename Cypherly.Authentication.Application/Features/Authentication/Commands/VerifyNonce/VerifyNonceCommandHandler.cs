﻿using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Token;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public class VerifyNonceCommandHandler(
    IUserRepository userRepository,
    INonceCacheService nonceCacheService,
    IJwtService jwtService,
    IVerifyNonceService verifyNonceService,
    IUnitOfWork unitOfWork,
    ILogger<VerifyNonceCommandHandler> logger)
    : ICommandHandler<VerifyNonceCommand, VerifyNonceDto>
{
    public async Task<Result<VerifyNonceDto>> Handle(VerifyNonceCommand cmd, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(cmd.UserId);

            if (user is null)
            {
                logger.LogWarning("User with ID: {ID} not found.", cmd.UserId);
                return Result.Fail<VerifyNonceDto>(Errors.General.NotFound(cmd.UserId));
            }

            var nonce = await nonceCacheService.GetNonceAsync(cmd.NonceId, cancellationToken);

            if (nonce is null)
            {
                logger.LogWarning("Nonce with ID: {ID} not found.", cmd.NonceId);
                return Result.Fail<VerifyNonceDto>(Errors.General.NotFound(cmd.NonceId));
            }

            var device = user.GetDevice(cmd.DeviceId);

            var isNonceValid = verifyNonceService.VerifyNonce(nonce.NonceValue, cmd.Nonce, device.PublicKey);

            if (!isNonceValid)
                return Result.Fail<VerifyNonceDto>(Errors.General.Unauthorized());

            var token = jwtService.GenerateToken(user.Id, cmd.DeviceId, user.GetUserClaims());
            device.AddRefreshToken();
            var refreshToken = device.GetActiveRefreshToken();

            var dto = VerifyNonceDto.Map(token, refreshToken!);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Exception occurred attempting to verify nonce for user with ID: {UserId} and Device ID: {DeviceId}.", cmd.UserId, cmd.DeviceId);
            return Result.Fail<VerifyNonceDto>(Errors.General.UnspecifiedError("An exception occured attempting to verify nonce."));
        }
    }
}