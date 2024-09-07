using Cypherly.UserManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Domain.Test.Unit.ValueObjectsTest
{
    public class UserTagTests
    {
        [Fact]
        public void Create_ShouldReturnValidUserTag_WhenGivenValidDisplayName()
        {
            // Arrange
            var displayName = "TestUser";

            // Act
            var userTag = UserTag.Create(displayName);

            // Assert
            userTag.Should().NotBeNull();
            userTag.Tag.Should().StartWith(displayName);
            userTag.Tag.Should().MatchRegex(@"^TestUser#\d{6}[A-Z]$"); // Ensures the format is "TestUser#000000A"
        }

        [Fact]
        public void Create_ShouldGenerateUniqueUserTags_ForDifferentInvocations()
        {
            // Arrange
            var displayName = "TestUser";

            // Act
            var userTag1 = UserTag.Create(displayName);
            var userTag2 = UserTag.Create(displayName);

            // Assert
            userTag1.Should().NotBeNull();
            userTag2.Should().NotBeNull();
            userTag1.Tag.Should().NotBe(userTag2.Tag); // Ensure that two invocations generate different tags
        }

        [Fact]
        public void Equals_ShouldReturnTrue_ForSameUserTagValue()
        {
            // Arrange
            var displayName = "TestUser";
            var userTag1 = UserTag.Create(displayName);
            var userTag2 = UserTag.Create(displayName);

            // Act
            var isEqual = userTag1.Equals(userTag2);

            // Assert
            isEqual.Should().BeFalse(); // Even though display names are the same, tags are unique.
        }

        [Fact]
        public void Equals_ShouldReturnFalse_ForDifferentUserTagValues()
        {
            // Arrange
            var displayName1 = "TestUser1";
            var displayName2 = "TestUser2";

            // Act
            var userTag1 = UserTag.Create(displayName1);
            var userTag2 = UserTag.Create(displayName2);

            // Assert
            userTag1.Tag.Should().NotBe(userTag2.Tag);
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent_ForTheSameUserTag()
        {
            // Arrange
            var displayName = "TestUser";
            var userTag = UserTag.Create(displayName);

            // Act
            var hashCode1 = userTag.GetHashCode();
            var hashCode2 = userTag.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }
    }
}
