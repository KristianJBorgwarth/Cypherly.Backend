using AutoMapper;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfileByIdQuery;

public class GetUserProfileQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepository;
    private readonly IMapper _fakeMapper;
    private readonly GetUserProfileQueryHandler _sut;

    public GetUserProfileQueryHandlerTest()
    {
        _fakeRepository = A.Fake<IUserProfileRepository>();
        _fakeMapper = A.Fake<IMapper>();
        var fakeLogger = A.Fake<ILogger<GetUserProfileQueryHandler>>();
        _sut = new GetUserProfileQueryHandler(_fakeRepository, _fakeMapper, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<GetUserProfileDto>(A<UserProfile>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occured while handling the request");
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<GetUserProfileDto>(A<UserProfile>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsUserProfile()
    {
        // Arrange
        var query = new Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);

        var dto = new GetUserProfileDto()
        {
            DisplayName = null,
            UserTag = userProfile.UserTag.Tag,
            Id = userProfile.Id,
            Username = userProfile.Username
        };

        A.CallTo(() => _fakeMapper.Map<GetUserProfileDto>(userProfile)).Returns(dto);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        A.CallTo(()=> _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeMapper.Map<GetUserProfileDto>(userProfile)).MustHaveHappenedOnceExactly();
    }
}