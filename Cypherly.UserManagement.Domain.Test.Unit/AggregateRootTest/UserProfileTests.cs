using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Domain.Test.Unit.AggregateRootTest
{
    public class UserProfileTests
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
            userProfile.DisplayName.Should().BeNull();
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
            userProfile.DisplayName.Should().BeNull();
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
            userProfile.DisplayName.Should().BeNull();
        }
    }
}
