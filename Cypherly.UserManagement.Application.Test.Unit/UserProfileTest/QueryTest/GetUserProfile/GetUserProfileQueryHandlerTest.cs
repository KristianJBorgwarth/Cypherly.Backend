using AutoMapper;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfile;

public class GetUserProfileQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepository;
    private readonly IProfilePictureService _fakeProfilePictureService;
    private readonly GetUserProfileQueryHandler _sut;

    public GetUserProfileQueryHandlerTest()
    {
        _fakeRepository = A.Fake<IUserProfileRepository>();
        _fakeProfilePictureService = A.Fake<IProfilePictureService>();
        var fakeLogger = A.Fake<ILogger<GetUserProfileQueryHandler>>();
        _sut = new(_fakeRepository, _fakeProfilePictureService, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occured while handling the request");
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsUserProfile()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);

        var dto = GetUserProfileDto.MapFrom(userProfile, "");

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        A.CallTo(()=> _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_And_Has_ProfilePicture_ReturnsUserProfile_With_ProfilePictureUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);
        A.CallTo(()=> _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).Returns(Result.Ok("presignedUrl"));


        var dto = GetUserProfileDto.MapFrom(userProfile, "presignedUrl");


        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("presignedUrl");
        A.CallTo(()=> _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_When_ProfilePictureServiceFails_Should_Return_UserProfile_With_EmptyUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);

        A.CallTo(()=> _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).Returns(Result.Fail<string>(Errors.General.UnspecifiedError("Failed to get presigned url")));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("");
        A.CallTo(()=> _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }
}