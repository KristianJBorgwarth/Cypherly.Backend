using System.Security.Claims;
using System.Text;
using Cypherly.Authentication.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cypherly.Authentication.Application.Services.Authentication;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    public string GenerateToken(Guid userId, string userEmail, List<UserClaim> userClaims)
    {
        var claims = new List<Claim>
        {
            new("sub", userEmail),
            new("jti", Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        claims.AddRange(userClaims.Select(uc => new Claim("role", uc.Claim.ClaimType.ToString())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.TokenLifeTimeInMinutes),
            SigningCredentials = creds,
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}