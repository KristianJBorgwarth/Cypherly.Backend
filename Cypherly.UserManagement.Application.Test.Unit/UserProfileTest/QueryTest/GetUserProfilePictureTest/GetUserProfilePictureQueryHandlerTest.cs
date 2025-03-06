using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfilePictureTest;

public class GetUserProfilePictureQueryHandlerTest
{
    private readonly IMinioProxyClient _fakeMinioProxyClient;
    private readonly GetUserProfilePictureQueryHandler _sut;

    public GetUserProfilePictureQueryHandlerTest()
    {
        _fakeMinioProxyClient = A.Fake<IMinioProxyClient>();
        var logger = A.Fake<ILogger<GetUserProfilePictureQueryValidator>>();
        _sut = new GetUserProfilePictureQueryHandler(_fakeMinioProxyClient, logger);
    }

    [Fact]
    public async Task Handle_Valid_Query_Should_Return_GetUserProfilePictureDto()
    {
        // Arrange
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = "https://example.com/profile.jpg" };

        A.CallTo(() => _fakeMinioProxyClient.GetImageFromMinioAsync(A<string>._, A<CancellationToken>._))
            .Returns(Result.Ok((new byte[] { 0x00 }, "image/jpeg")));

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

        A.CallTo(() => _fakeMinioProxyClient.GetImageFromMinioAsync(A<string>._, A<CancellationToken>._))
            .Returns(Task.FromResult<(byte[] image, string imageType)?>(null));


        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Exception_Should_Return_Fail_Result()
    {
        // Arrange
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = "https://example.com/profile.jpg" };

        A.CallTo(() => _fakeMinioProxyClient.GetImageFromMinioAsync(A<string>._, A<CancellationToken>._))
            .Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}