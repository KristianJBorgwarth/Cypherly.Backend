﻿using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Caching.LoginNonce;
using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Application.Features.Device.Commands.Create;

public sealed record CreateDeviceCommand : ICommand<CreateDeviceDto>
{
    public required Guid UserId { get; init; }
    public required Guid LoginNonceId { get; init; }
    public required string LoginNonce { get; init; }
    public required string DeviceAppVersion { get; init; }
    public required DeviceType DeviceType { get; init; }
    public required DevicePlatform DevicePlatform { get; init; }
    public required string Base64DevicePublicKey { get; init; }
}