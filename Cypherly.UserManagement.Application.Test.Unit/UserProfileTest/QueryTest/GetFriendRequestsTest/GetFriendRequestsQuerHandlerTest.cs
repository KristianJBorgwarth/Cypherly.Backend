using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetFriendRequestsTest;

public class GetFriendRequestsQuerHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly GetFriendRequestsQueryHandler _sut;

    public GetFriendRequestsQuerHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        var logger = A.Fake<ILogger<GetFriendRequestsQueryHandler>>();
        _sut = new GetFriendRequestsQueryHandler(_fakeRepo, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_Result_Ok()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "Kristian", UserTag.Create("Kristian"));

        var query = new GetFriendRequestsQuery()
        {
            UserId = user.Id,
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Query_Should_Return_Result_Fail()
    {
        // Arrange
        var query = new GetFriendRequestsQuery()
        {
            UserId = Guid.NewGuid(),
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Given_Throws_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        var query = new GetFriendRequestsQuery()
        {
            UserId = Guid.NewGuid(),
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}