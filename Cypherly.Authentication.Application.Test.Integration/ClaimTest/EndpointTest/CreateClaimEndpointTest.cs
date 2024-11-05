using System.Net;
using System.Net.Http.Json;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using TestUtilities.Attributes;

namespace Cypherly.Authentication.Application.Test.Integration.ClaimTest.EndpointTest;

public class CreateClaimEndpointTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async void Given_Valid_Request_Should_Create_Claim_And_Return_200()
    {
        // Arrange
        var request = new CreateClaimCommand()
        {
            ClaimType = "Admin",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Claim/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Claim.Should().HaveCount(1);
    }

    [SkipOnGitHubFact]
    public async void Given_Invalid_Request_Should_Return_400()
    {
        // Arrange
        var request = new CreateClaimCommand()
        {
            ClaimType = "",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Claim/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.Claim.Should().HaveCount(0);    }
}