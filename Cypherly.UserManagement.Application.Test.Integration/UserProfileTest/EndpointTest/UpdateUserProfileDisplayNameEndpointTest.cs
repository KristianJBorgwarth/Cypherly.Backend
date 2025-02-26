using System.Net;
using System.Net.Http.Json;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestUtilities.Attributes;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class UpdateUserProfileDisplayNameEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Given_Valid_Request_Should_Update_UserProfile_DisplayName_And_Return_200OK()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "test",
            Id = userprofile.Id
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/displayname", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.UserProfile.AsNoTracking().FirstOrDefault(u=> u.Id == userprofile.Id)!.DisplayName.Should().Be("test");
        Db.OutboxMessage.Should().HaveCount(1);
    }

    [SkipOnGitHubFact]
    public async Task Given_Invalid_Request_Should_Return_400BadRequest()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "",
            Id = Guid.NewGuid()
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/displayname", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.UserProfile.AsNoTracking().FirstOrDefault(u=> u.Id == userprofile.Id)!.DisplayName.Should().Be("david");
    }
}