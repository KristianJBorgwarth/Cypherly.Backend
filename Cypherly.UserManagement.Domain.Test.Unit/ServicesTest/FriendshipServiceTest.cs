using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Cypherly.UserManagement.Domain.Services;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Domain.Test.Unit.ServicesTest;

public class FriendshipServiceTest
{
    private readonly IUserProfileLifecycleService _profileLifecycleService = new UserProfileLifecycleLifecycleService();
    private readonly IFriendshipService _sut = new FriendshipService();

        [Fact]
        public void CreateFriendship_ShouldReturnSuccess_WhenValidProfilesAreGiven()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var friendId = Guid.NewGuid();
            var userProfile = _profileLifecycleService.CreateUserProfile(userId, "User1");
            var friendProfile = _profileLifecycleService.CreateUserProfile(friendId, "User2");

            // Act
            var result = _sut.CreateFriendship(userProfile, friendProfile);

            // Assert
            result.Success.Should().BeTrue();
            userProfile.FriendshipsInitiated.Should().HaveCount(1);
        }

        [Fact]
        public void CreateFriendship_ShouldReturnFailure_WhenFriendshipAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var friendId = Guid.NewGuid();
            var userProfile = _profileLifecycleService.CreateUserProfile(userId, "User1");
            var friendProfile = _profileLifecycleService.CreateUserProfile(friendId, "User2");

            _sut.CreateFriendship(userProfile, friendProfile); // Create the friendship initially

            // Act
            var result = _sut.CreateFriendship(userProfile, friendProfile); // Try creating it again

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Be("Friendship already exists");
        }

        [Fact(Skip = "EF Core handles population, use integration test")]
        public void AcceptFriendship_ShouldReturnSuccess_WhenFriendshipIsPending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var friendId = Guid.NewGuid();
            var userProfile = _profileLifecycleService.CreateUserProfile(userId, "User1");
            var friendProfile = _profileLifecycleService.CreateUserProfile(friendId, "User2");

            _sut.CreateFriendship(friendProfile, userProfile); // Friend initiates friendship
            _sut.CreateFriendship(userProfile, friendProfile);

            // Act
            var result = _sut.AcceptFriendship(userProfile, friendProfile.UserTag.Tag);

            // Assert
            result.Success.Should().BeTrue();
            userProfile.FriendshipsReceived.First().Status.Should().Be(FriendshipStatus.Accepted);
            userProfile.DomainEvents.Should().ContainSingle(e => e is FriendshipAcceptedEvent);
        }

        [Fact]
        public void AcceptFriendship_ShouldReturnFailure_WhenFriendshipIsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userProfile = _profileLifecycleService.CreateUserProfile(userId, "User1");

            // Act
            var result = _sut.AcceptFriendship(userProfile, "NonExistentTag");

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Be("Friendship not found");
        }
}