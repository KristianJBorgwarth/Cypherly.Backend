using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EndpointTest;

public class VerifyUserEndpointTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Valid_Request_Should_Verify_User_And_Return_200_Ok()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@email.dk"), Password.Create("lolwortks?293K"), false);
        user.AddVerificationCode(UserVerificationCodeType.EmailVerification);
        await Db.User.AddAsync(user);
        await Db.Claim.AddAsync(new Claim(Guid.NewGuid(), "user"));
        await Db.SaveChangesAsync();

        var req = new VerifyUserCommand()
        {
            UserId = user.Id,
            VerificationCode = user.GetActiveVerificationCode(UserVerificationCodeType.EmailVerification)!.Code.Value
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/user/verify", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.User.AsNoTracking().FirstOrDefault(u => u.Id == user.Id)!.IsVerified.Should().BeTrue();
        Db.VerificationCode.AsNoTracking().FirstOrDefault(vc => vc.UserId == user.Id)!.Code.IsUsed.Should().BeTrue();
        Db.OutboxMessage.Should().HaveCount(1);
    }

    [Fact]
    public async void Invalid_Request_Should_Return_400_BadRequest()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@email.dk"), Password.Create("lolwortks?293K"), false);
        user.AddVerificationCode(UserVerificationCodeType.EmailVerification);
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var req = new VerifyUserCommand()
        {
            UserId = user.Id,
            VerificationCode = "InvalidCode"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/user/verify", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.User.AsNoTracking().FirstOrDefault(u => u.Id == user.Id)!.IsVerified.Should().BeFalse();
        Db.VerificationCode.AsNoTracking().FirstOrDefault(vc => vc.UserId == user.Id)!.Code.IsUsed.Should().BeFalse();
        Db.OutboxMessage.Should().HaveCount(0);
    }
}