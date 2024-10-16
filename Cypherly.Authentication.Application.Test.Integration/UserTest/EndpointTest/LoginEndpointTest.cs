using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EndpointTest;

public class LoginEndpointTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Given_Valid_Login_Request_Should_Return_200_And_Token()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), true);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "TestPassword?123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/user/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.RefreshToken.Should().HaveCount(1);
    }

    [Fact]
    public async Task Given_Invalid_Login_Request_Should_Return_400()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("TestPassword?123"), false); // Unverified user

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = user.Email.Address,
            Password = "TestPassword?123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/user/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.RefreshToken.Should().HaveCount(0);
    }
}