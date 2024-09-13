using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.API.Utilities;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class GetUserProfileByIdEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async void Given_Valid_Request_Should_Return_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByIdQuery { UserProfileId = userProfile.Id };

        // Act
        var response = await Client.GetFromJsonAsync<Envelope<GetUserProfileByIdDto>>($"/api/userprofile?UserProfileId={userProfile.Id}");

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().Be(userProfile.Id);
        response.Result.Username.Should().Be(userProfile.Username);
        response.Result.UserTag.Should().Be(userProfile.UserTag.Tag);
    }

    [Fact]
    public async void Given_Invalid_Request_Should_Return_NotFound()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByIdQuery { UserProfileId = userProfile.Id };

        // Act
        var response = await Client.GetAsync($"/api/userprofile?UserProfileId={Guid.NewGuid()}"); //WRONG ID

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}