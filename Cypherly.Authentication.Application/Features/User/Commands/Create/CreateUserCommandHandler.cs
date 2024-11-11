﻿using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using Cypherly.Domain.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUserLifeCycleServices userLifeCycleServices,
    IDeviceService deviceService,
    IUnitOfWork unitOfWork,
    IRequestClient<CreateUserProfileRequest> requestClient,
    ILogger<CreateUserCommandHandler> logger)
    : ICommandHandler<CreateUserCommand, CreateUserDto>
{
    public async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if( await DoesEmailExist(email: request.Email))
                return Result.Fail<CreateUserDto>(Errors.General.UnspecifiedError("An account already exists with that email"));

            var userResult = userLifeCycleServices.CreateUser(request.Email, request.Password);

            if (userResult.Success is false || userResult.Value is null)
                return Result.Fail<CreateUserDto>(userResult.Error);

            var device = deviceService.RegisterDevice(userResult.Value, request.DeviceName, request.DevicePublicKey, request.DeviceAppVersion, request.DeviceType, request.DevicePlatform);
            await userRepository.CreateAsync(userResult.Value);


            var createProfileResult = await CreateProfile(userResult.Value.Id, request.Username);

            if (createProfileResult.Success is false)
                return Result.Fail<CreateUserDto>(createProfileResult.Error);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = CreateUserDto.Map(userResult.Value, device);

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occured while attempting to create a user");
            return Result.Fail<CreateUserDto>(Errors.General.UnspecifiedError("Exception occured while attempting to create a user. Check logs for more information"));
        }
    }

    private async Task<bool> DoesEmailExist(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);
        return user is not null;
    }

    private async Task<Result> CreateProfile(Guid userId, string username)
    {
        var createProfileRequest = new CreateUserProfileRequest(userId, username);
        var response = await requestClient.GetResponse<CreateUserProfileResponse>(createProfileRequest);

        if (response.Message.IsSuccess)
            return Result.Ok();

        logger.LogError("Failed to create user profile");
        return Result.Fail(Errors.General.UnspecifiedError(response.Message.Error!));

    }
}