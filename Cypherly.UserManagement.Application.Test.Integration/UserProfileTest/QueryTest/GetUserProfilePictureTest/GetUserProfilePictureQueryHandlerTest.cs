using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Application.Test.Integration.Setup.Helpers;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.QueryTest.GetUserProfilePictureTest;

public class GetUserProfilePictureQueryHandlerTest : IntegrationTestBase
{
    private readonly GetUserProfilePictureQueryHandler _sut;
    private readonly IProfilePictureService _profilePictureService;
    public GetUserProfilePictureQueryHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var minoProxyClient = scope.ServiceProvider.GetRequiredService<IMinioProxyClient>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserProfilePictureQueryValidator>>();
        _sut = new GetUserProfilePictureQueryHandler(minoProxyClient, logger);
        _profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
    }

    [Fact]
    public async Task Handle_Valid_Query_Should_Return_GetUserProfilePictureDto()
    {
        // Arrange
        var profilePic = FormFileHelper.CreateFormFile(
            Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Cypherly.UserManagement.Application.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png");

        var url = await _profilePictureService.UploadProfilePictureAsync(profilePic, Guid.NewGuid());

        var presignedUrl = await _profilePictureService.GetPresignedProfilePictureUrlAsync(url);

        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = presignedUrl.Value! };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Invalid_Query_Should_Return_Fail_Result()
    {
        // Arrange
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = "invalid_url" };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}