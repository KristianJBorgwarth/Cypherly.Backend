﻿using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Services.Authentication;

public interface IJwtService
{
    string GenerateToken(Guid UserId, string UserEmail, List<UserClaim> claims);
    string GenerateToken2(Guid UserId, Guid DeviceId, List<UserClaim> userClaims);
}