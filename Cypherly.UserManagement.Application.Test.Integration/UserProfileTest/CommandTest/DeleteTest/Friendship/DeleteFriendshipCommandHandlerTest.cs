using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.CommandTest.DeleteTest.Friendship;

public class DeleteFriendshipCommandHandlerTest : IntegrationTestBase
{
    private readonly DeleteFriendshipCommandHandler _sut;
    public DeleteFriendshipCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IFriendshipService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeleteFriendshipCommandHandler>>();
        _sut = new(repo, unitOfWork, userProfileService, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Delete_Friendship()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser",UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();
        Db.Friendship.Should().HaveCount(1);

        var cmd = new DeleteFriendshipCommand()
        {
            Id = userProfile.Id,
            FriendTag = friendProfile.UserTag.Tag
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        Db.Friendship.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_UserProfileId_Should_Return_NotFound()
    {
        // Arrange
        var cmd = new DeleteFriendshipCommand()
        {
            Id = Guid.NewGuid(),
            FriendTag = "FriendUser"
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_Given_Command_With_No_Friendship_Should_Return_Result_Fail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser",UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        //NOTE: No friendship added
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();

        var cmd = new DeleteFriendshipCommand()
        {
            Id = userProfile.Id,
            FriendTag = friendProfile.UserTag.Tag
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Friendship not found");
        Db.Friendship.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_Given_Command_With_Null_FriendTag_Should_Throw_InvalidOperationException()
    {

        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var cmd = new DeleteFriendshipCommand()
        {
            Id = userprofile.Id,
            FriendTag = null
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);


        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An exception occured while attempting to delete a friendship.");
    }
}