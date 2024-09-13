using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.QueryTest.GetFriendsTest;

public class GetFriendsQueryHandlerTest : IntegrationTestBase
{
    private readonly GetFriendsQueryHandler _sut;
    public GetFriendsQueryHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetFriendsQueryHandler>>();

        _sut = new(repo, logger);
    }

    [Fact]
    public async void Handle_Given_Valid_Query_Should_Return_Relevant_Friends()
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

        var query = new GetFriendsQuery { UserProfileId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.First().Username.Should().Be("John");
    }

    [Fact]
    public async void Handle_Given_Invalid_Query_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetFriendsQuery { UserProfileId = Guid.NewGuid() };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async void Handle_Given_Empty_Friends_Should_Return_Empty_List()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.Add(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetFriendsQuery { UserProfileId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(0);
    }
}