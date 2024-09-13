using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetFriends;

public class GetFriendsQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly GetFriendsQueryHandler _sut;

    public GetFriendsQueryHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        var fakeLogger = A.Fake<ILogger<GetFriendsQueryHandler>>();
        _sut = new GetFriendsQueryHandler(_fakeRepo, fakeLogger);
    }

    [Fact]
    public async void Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetFriendsQuery { UserProfileId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetFriendsQuery { UserProfileId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occurred while attempting to retrieve friends.");
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_ReturnsFriends()
    {
        // Arrange
        var query = new GetFriendsQuery { UserProfileId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "Eric", UserTag.Create("Eric"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).Returns(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }
}