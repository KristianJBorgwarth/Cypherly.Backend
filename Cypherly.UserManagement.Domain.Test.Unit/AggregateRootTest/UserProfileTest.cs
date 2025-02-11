using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Domain.Test.Unit.AggregateRootTest
{
    public class UserProfileTest
    {
        [Fact]
        public void SetProfilePictureUrl_ShouldUpdateProfilePictureUrl()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            userProfile.SetProfilePictureUrl(profilePictureUrl);

            // Assert
            userProfile.ProfilePictureUrl.Should().Be(profilePictureUrl);
            userProfile.DomainEvents.Should().HaveCount(1);
        }

        [Fact]
        public void SetDisplayName_ShouldSucceed_WhenValidDisplayName()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var validDisplayName = "ValidName";

            // Act
            var result = userProfile.SetDisplayName(validDisplayName);

            // Assert
            result.Success.Should().BeTrue();
            userProfile.DisplayName.Should().Be(validDisplayName);
            userProfile.DomainEvents.Should().HaveCount(1);
        }

        [Fact]
        public void SetDisplayName_ShouldFail_WhenDisplayNameTooShort()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var shortDisplayName = "ab"; // Less than 3 characters

            // Act
            var result = userProfile.SetDisplayName(shortDisplayName);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("should be at least 3.");
            userProfile.DisplayName.Should().Be("TestUser");
            userProfile.DomainEvents.Should().HaveCount(0);
        }

        [Fact]
        public void SetDisplayName_ShouldFail_WhenDisplayNameTooLong()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var longDisplayName = new string('a', 21); // More than 20 characters

            // Act
            var result = userProfile.SetDisplayName(longDisplayName);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("should not exceed 20.");
            userProfile.DisplayName.Should().Be("TestUser");
            userProfile.DomainEvents.Should().HaveCount(0);

        }

        [Fact]
        public void SetDisplayName_ShouldFail_WhenDisplayNameHasInvalidCharacters()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var invalidDisplayName = "Invalid@Name"; // Contains invalid characters

            // Act
            var result = userProfile.SetDisplayName(invalidDisplayName);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("not valid");
            userProfile.DisplayName.Should().Be("TestUser");
            userProfile.DomainEvents.Should().HaveCount(0);

        }

        [Fact]
        public void AddFriendship_ShouldFail_WhenAddingSelfAsFriend()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));

            // Act
            var result = userProfile.AddFriendship(userProfile);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("Cannot add self as friend");
        }

        [Fact]
        public void AddFriendship_ShouldSucceed_WhenAddingValidFriend()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

            // Act
            var result = userProfile.AddFriendship(friendProfile);

            // Assert
            result.Success.Should().BeTrue();
            userProfile.FriendshipsInitiated.Should().ContainSingle(f => f.FriendProfileId == friendProfile.Id);
        }

        [Fact]
        public void AddFriendship_ShouldFail_WhenFriendshipAlreadyExists()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

            // Act
            userProfile.AddFriendship(friendProfile); // First time, should succeed
            var result = userProfile.AddFriendship(friendProfile); // Second time, should fail

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("Friendship already exists");
        }

        [Fact]
        public void AddFriendship_ShouldFail_WhenFriendshipExistsInReceivedList()
        {
            // Arrange
            var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
            var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

            // Simulate that friend has already initiated a friendship with userProfile
            friendProfile.AddFriendship(userProfile);
            userProfile.AddFriendship(friendProfile);

            // Act
            var result = userProfile.AddFriendship(friendProfile); // Should fail, because friendship is in received list

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("Friendship already exists");
        }
    }
}
