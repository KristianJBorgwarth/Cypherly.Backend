﻿using System.Security.Claims;
using System.Text;
using Cypherly.Authentication.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cypherly.Authentication.Application.Features.Authentication.Token;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    public string GenerateToken(Guid userId, Guid deviceId, List<UserClaim> userClaims)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("sub", deviceId.ToString()),
            new("jti", Guid.NewGuid().ToString()),
        };

        claims.AddRange(userClaims.Select(uc => new System.Security.Claims.Claim(ClaimTypes.Role, uc.Claim.ClaimType.ToString())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new(claims),
            Expires = DateTime.UtcNow.AddDays(jwtSettings.Value.TokenLifeTimeInMinutes),
            SigningCredentials = creds,
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}