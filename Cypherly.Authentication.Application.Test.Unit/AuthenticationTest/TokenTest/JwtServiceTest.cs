using System.Reflection;
using System.Security.Claims;
using Cypherly.Authentication.Application.Features.Authentication.Token;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Claim = Cypherly.Authentication.Domain.Aggregates.Claim;

namespace Cypherly.Authentication.Application.Test.Unit.AuthenticationTest.TokenTest;

public class JwtServiceTest
{
    private readonly IJwtService _jwtService;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public JwtServiceTest()
    {
        _jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "superduperextremetesterinosecretirnosecret_",
            Issuer = "testissuer",
            Audience = "testaudience",
            TokenLifeTimeInMinutes = 10,
        });

        _jwtService = new JwtService(_jwtSettings);
    }

    [Fact]
    public async void GenerateToken_Should_Return_Token_With_Valid_Ids()
    {
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test@password?23K"), true);

        var device = new Device(Guid.NewGuid(), "testKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device);

        var claim = new Claim(Guid.NewGuid(), "user");
        var userClaim = new UserClaim(Guid.NewGuid(), user.Id, claim.Id);

        var claimProperty = typeof(UserClaim).GetProperty("Claim", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        claimProperty.SetValue(userClaim, claim);

        var userClaims = new List<UserClaim> { userClaim };

        // Act
        var token = _jwtService.GenerateToken2(user.Id, device.Id, userClaims);

        // Decode the token
        var jwtHandler = new JsonWebTokenHandler();
        var decodedToken = jwtHandler.ReadJsonWebToken(token);

        // Assert: Verify token claims
        decodedToken.Claims.First(c => c.Type == "sub").Value.Should().Be(device.Id.ToString());
        decodedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be(user.Id.ToString());
        decodedToken.Claims.First(c => c.Type == "jti").Value.Should().NotBeNullOrEmpty();
        var roleClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        roleClaim.Should().NotBeNull();
        roleClaim!.Value.Should().Be(userClaim.Claim.ClaimType);
    }
}