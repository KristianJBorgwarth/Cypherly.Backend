using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.CommandTest.UpdateTest.AcceptFriendship;

public class AcceptFriendshipCommandHandlerTest : IntegrationTestBase
{
    private readonly AcceptFriendshipCommandHandler _sut;
    public AcceptFriendshipCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AcceptFriendshipCommandHandler>>();

        _sut = new AcceptFriendshipCommandHandler(repo, service, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Valid_Command_Should_Accept_Friendship()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "Dave", UserTag.Create("Dave"));
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        Db.Friendship.Add(new Friendship(Guid.NewGuid(), userProfile.Id, friendProfile.Id));
        await Db.SaveChangesAsync();

        var command = new AcceptFriendshipCommand()
        {
            FriendTag = userProfile.UserTag.Tag,
            Id = friendProfile.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        var updatedUserProfile = Db.UserProfile.AsNoTracking().First(u => u.Id == userProfile.Id).FriendshipsInitiated
            .First().Status.Should().Be(FriendshipStatus.Accepted);
        var friendUserProfile = Db.UserProfile.AsNoTracking().First(u => u.Id == friendProfile.Id).FriendshipsReceived
            .First().Status.Should().Be(FriendshipStatus.Accepted);
    }

    [Fact]
    public async Task Handle_Command_Invalid_Id_Should_Return_Result_Fail()
    {
        // Arrange
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "eric",
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_Command_Invalid_FriendTag_Should_Return_Result_Fail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "Dave", UserTag.Create("Dave"));
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        Db.Friendship.Add(new Friendship(Guid.NewGuid(), userProfile.Id, friendProfile.Id));
        await Db.SaveChangesAsync();

        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "invalid",
            Id = friendProfile.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("Friendship not found");
    }
}