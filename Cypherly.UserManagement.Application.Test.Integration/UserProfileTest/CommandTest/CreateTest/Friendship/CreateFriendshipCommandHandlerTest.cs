using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.CommandTest.CreateTest.Friendship;

public class CreateFriendshipCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateFriendshipCommandHandler _sut;
    public CreateFriendshipCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var service = scope.ServiceProvider.GetRequiredService<IFriendshipService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateFriendshipCommandHandler>>();
        _sut = new CreateFriendshipCommandHandler(repo, service, unitOfWork, logger);
    }

    [Fact]
    public async void Handle_Given_Valid_Command_Should_Create_Friendship_And_Return_Result_Ok()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        Db.Friendship.Should().HaveCount(1);
        var userProfileResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == user.Id);
        userProfileResult!.FriendshipsInitiated.Should().HaveCount(1);
        var friendResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == friend.Id);
        friendResult!.FriendshipsReceived.Should().HaveCount(1);
    }

    [Fact]
    public async void Handle_Given_Existing_Friendship_Should_Return_Error()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        Db.Friendship.Add(new(Guid.NewGuid(), user.Id, friend.Id));
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Friendship already exists");
    }

    [Fact]
    public async void Handle_Given_Invalid_UserId_Should_Return_NotFound()
    {
        // Arrange
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.Add(friend);
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            Id = Guid.NewGuid() // Non-existent user
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async void Handle_Given_Invalid_FriendTag_Should_Return_NotFound()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = "InvalidTag", // Non-existent friend
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
}