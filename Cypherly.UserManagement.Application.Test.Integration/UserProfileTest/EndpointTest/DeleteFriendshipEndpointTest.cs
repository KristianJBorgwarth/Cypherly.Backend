using System.Net;
using System.Net.Http.Json;
using Cypherly.API.Responses;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using TestUtilities.Attributes;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class DeleteFriendshipEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Given_Valid_Request_Should_Delete_Friendship_And_Return_200OK()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser",UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();
        Db.Friendship.Should().HaveCount(1);

        var cmd = new DeleteFriendshipCommand
        {
            Id = userProfile.Id,
            FriendTag = friendProfile.UserTag.Tag
        };

        // Act
        var encodedFriendTag = Uri.EscapeDataString(cmd.FriendTag);
        var response = await Client.DeleteAsync($"api/userprofile/friendship?Id={cmd.Id}&friendTag={encodedFriendTag}");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<Envelope>();
        result.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Friendship.Should().HaveCount(0);
    }

    [SkipOnGitHubFact]
    public async Task Given_Invalid_Request_Should_Return_400BadRequest()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser",UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();
        Db.Friendship.Should().HaveCount(1);

        var cmd = new DeleteFriendshipCommand
        {
            Id = userProfile.Id,
            FriendTag = "InvalidFriendTag"
        };

        // Act
        var encodedFriendTag = Uri.EscapeDataString(cmd.FriendTag);
        var response = await Client.DeleteAsync($"api/userprofile/friendship?Id={cmd.Id}&friendTag={encodedFriendTag}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.Friendship.Should().HaveCount(1);
    }
}