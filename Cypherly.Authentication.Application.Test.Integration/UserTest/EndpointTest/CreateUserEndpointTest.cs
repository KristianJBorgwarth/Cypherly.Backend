using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using FluentAssertions;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.EndpointTest;

public class CreateUserEndpointTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async void Valid_Request_Should_Create_User_And_Return_200_Ok()
    {
        // Arrange
        var req = new CreateUserCommand()
        {
            Username = "TestUser",
            Email = "test@email.dk",
            Password = "TestPassword3?",
            DeviceAppVersion = "1.0",
            DeviceName = "deviceName",
            DevicePlatform = Domain.Enums.DevicePlatform.Android,
            DevicePublicKey = "devicePublicKey"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/user", req);

        // Assert
        Harness.Published.Select<CreateUserProfileRequest>().Where(cr=> cr.Context.Message.Username == "TestUser").Should().HaveCount(1);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.User.Count().Should().Be(1);
        Db.User.First().Email.Address.Should().Be(req.Email);
        Db.OutboxMessage.Count().Should().Be(1);
    }

    [Fact]
    public async void Invalid_Request_Should_Return_400_BadRequest()
    {
        // Arrange
        var req = new CreateUserCommand()
        {
            Username = "TestUser",
            Email = "testemail.dk",
            Password = "TestPassword3?",
            DeviceAppVersion = "1.0",
            DeviceName = "deviceName",
            DevicePlatform = Domain.Enums.DevicePlatform.Android,
            DevicePublicKey = "devicePublicKey"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/user", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.User.Count().Should().Be(0);
        Db.OutboxMessage.Count().Should().Be(0);
    }
}