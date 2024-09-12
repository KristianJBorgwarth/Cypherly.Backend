using System.Net;
using System.Net.Http.Json;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class CreateFriendshipEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async void Given_Valid_Request_Should_Create_Friendship()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            UserId = user.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/userprofile/friendship", command);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Friendship.Should().HaveCount(1);
        var userProfileResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == user.Id);
        userProfileResult!.FriendshipsInitiated.Should().HaveCount(1);
        var friendResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == friend.Id);
        friendResult!.FriendshipsRecieved.Should().HaveCount(1);
    }

    [Fact]
    public async void Given_Invalid_Request_Should_Return_Error()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        // Simulate existing friendship
        Db.Friendship.Add(new(Guid.NewGuid(), user.Id, friend.Id));
        await Db.SaveChangesAsync();

        var request = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            UserId = user.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/userprofile/friendship", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.Friendship.Should().HaveCount(1);
    }
}