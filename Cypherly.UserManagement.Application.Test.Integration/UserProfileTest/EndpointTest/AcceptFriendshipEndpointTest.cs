using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cypherly.Authentication.API.Utilities;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class AcceptFriendshipEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Given_Valid_Request_Should_Accept_Friendship_And_Return_200OK()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "Dave", UserTag.Create("Dave"));
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        Db.Friendship.Add(new(Guid.NewGuid(), userProfile.Id, friendProfile.Id));
        await Db.SaveChangesAsync();

        var command = new AcceptFriendshipCommand()
        {
            FriendTag = userProfile.UserTag.Tag,
            UserId = friendProfile.Id
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/friendship", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUserProfile = Db.UserProfile.AsNoTracking().First(u => u.Id == userProfile.Id).FriendshipsInitiated
            .First().Status.Should().Be(FriendshipStatus.Accepted);
        var friendUserProfile = Db.UserProfile.AsNoTracking().First(u => u.Id == friendProfile.Id).FriendshipsReceived
            .First().Status.Should().Be(FriendshipStatus.Accepted);
    }

    [Fact]
    public async Task Given_Invalid_Request_Should_Return_400BadRequest()
    {
        // Arrange
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "eric",
            UserId = Guid.NewGuid()
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/friendship", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}