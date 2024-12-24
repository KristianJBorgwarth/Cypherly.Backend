﻿using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.RefreshTokens;

public class RefreshTokensCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IAuthenticationService authService,
    ILogger<RefreshTokensCommandHandler> logger) 
    : ICommandHandler<RefreshTokensCommand, RefreshTokensDto>
{
    
    public async Task<Result<RefreshTokensDto>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                logger.LogCritical("User with {UserId} not found", request.UserId);
                return Result.Fail<RefreshTokensDto>(Errors.General.NotFound(request.UserId));
            }

            var isTokenValid = authService.VerifyRefreshToken(user, request.DeviceId, request.RefreshToken);
            if (isTokenValid is false)
            {
                return Result.Fail<RefreshTokensDto>(Errors.General.UnspecifiedError("Invalid refresh token"));
            }
            
            var refreshToken = authService.GenerateRefreshToken(user, request.DeviceId);
            
            var accessToken = jwtService.GenerateToken(user.Id, user.Email.Address, user.GetUserClaims());

            var dto = RefreshTokensDto.Map(accessToken, refreshToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Exception occurred while refreshing tokens for user {UserId} with DeviceId {DeviceId} and RefreshToken {RefreshToken}", request.UserId, request.DeviceId, request.RefreshToken);
            
            return Result.Fail<RefreshTokensDto>(Errors.General.UnspecifiedError("Error occured while refreshing tokens"));
        }
    }
}