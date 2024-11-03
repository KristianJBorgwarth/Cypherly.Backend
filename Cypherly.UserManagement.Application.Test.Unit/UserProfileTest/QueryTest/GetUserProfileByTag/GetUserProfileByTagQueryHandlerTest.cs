using AutoMapper;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandlerTest
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserBlockingService _userBlockingService;
    private readonly IProfilePictureService _profilePictureService;
    private readonly IMapper _mapper;

    private readonly GetUserProfileByTagQueryHandler _sut;

    public GetUserProfileByTagQueryHandlerTest()
    {
        _userProfileRepository = A.Fake<IUserProfileRepository>();
        _userBlockingService = A.Fake<IUserBlockingService>();
        _profilePictureService = A.Fake<IProfilePictureService>();
        _mapper = A.Fake<IMapper>();
        var logger = A.Fake<ILogger<GetUserProfileByTagQueryHandler>>();

        _sut = new(_userProfileRepository, _userBlockingService, _profilePictureService, _mapper, logger);
    }

    [Fact]
    public async Task Handle_When_RequestingUser_Does_Not_Exist_Returns_Result_Fail()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = "TestTag"
        };

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.Id))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_When_UserProfile_Is_Blocked_Returns_Result_Empty()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = "TestTag"
        };

        var requestingUser = new UserProfile();
        var userProfile = new UserProfile();

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.Id)).Returns(requestingUser);
        A.CallTo(() => _userProfileRepository.GetByUserTag(request.Tag)).Returns(userProfile);
        A.CallTo(() => _userBlockingService.IsUserBloccked(requestingUser, userProfile))!.Returns(true);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_When_UserProfile_Does_Not_Exist_Return_Empty_Result()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = "TestTag"
        };

        var requestingUser = new UserProfile();

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.Id)).Returns(requestingUser);
        A.CallTo(() => _userProfileRepository.GetByUserTag(request.Tag))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_When_Repo_Throws_Exception_Returns_Result_Fail()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            Id = Guid.NewGuid(),
            Tag = "TestTag"
        };

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.Id)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}