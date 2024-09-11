using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EndpointTest;

public class VerifyUserEndpointTest : IntegrationTestBase
{
    public VerifyUserEndpointTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
    }

    [Fact]
    public async void Valid_Request_Should_Verify_User_And_Return_200_Ok()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@email.dk"), Password.Create("lolwortks?293K"), false);
        user.SetVerificationCode();
        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var req = new VerifyUserCommand()
        {
            UserId = user.Id,
            VerificationCode = user.VerificationCode!.Code
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/user/verify", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.User.AsNoTracking().FirstOrDefault(u => u.Id == user.Id)!.IsVerified.Should().BeTrue();
        Db.VerificationCode.AsNoTracking().FirstOrDefault(vc => vc.UserId == user.Id)!.IsUsed.Should().BeTrue();
    }

    [Fact]
    public async void Invalid_Request_Should_Return_400_BadRequest()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@email.dk"), Password.Create("lolwortks?293K"), false);
        user.SetVerificationCode();
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
        Db.VerificationCode.AsNoTracking().FirstOrDefault(vc => vc.UserId == user.Id)!.IsUsed.Should().BeFalse();
    }
}