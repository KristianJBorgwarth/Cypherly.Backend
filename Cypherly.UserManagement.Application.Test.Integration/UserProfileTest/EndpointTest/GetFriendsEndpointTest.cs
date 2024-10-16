using System.Net;
using System.Net.Http.Json;
using Cypherly.API.Responses;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using TestUtilities.Attributes;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class GetFriendsEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async void GetFriendsEndpoint_Should_Return_Friends()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        var friendProfile2 = new UserProfile(Guid.NewGuid(), "Jane", UserTag.Create("Jane"));
        var friendship = new Friendship(Guid.NewGuid(), userProfile.Id, friendProfile.Id);
        var friendship2 = new Friendship(Guid.NewGuid(), userProfile.Id, friendProfile2.Id);
        //Only accept the first friendship
        friendship.AcceptFriendship();
        Db.Add(userProfile);
        Db.Add(friendProfile);
        Db.Add(friendship);
        Db.Add(friendProfile2);
        Db.Add(friendship2);
        await Db.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/userprofile/friendships?UserProfileId={userProfile.Id}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Envelope<List<GetFriendsDto>>>();
        result!.Result[0].Username.Should().Be("John");
    }

    [SkipOnGitHubFact]
    public async void GetFriendsEndpoint_Invalid_Id_Should_Return_BadRequest()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.Add(userProfile);
        await Db.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/userprofile/friendships?UserProfileId={Guid.NewGuid()}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [SkipOnGitHubFact]
    public async void GetFriendsEndpoint_Should_Return_EmptyList()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.Add(userProfile);
        await Db.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/userprofile/friendships?UserProfileId={userProfile.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}