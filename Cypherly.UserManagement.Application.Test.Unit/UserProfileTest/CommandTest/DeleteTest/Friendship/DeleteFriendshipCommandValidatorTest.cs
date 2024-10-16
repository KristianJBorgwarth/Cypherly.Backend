using Xunit;
using FluentAssertions;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.DeleteTest.Friendship
{
    public class DeleteFriendshipCommandValidatorTests
    {
        private readonly DeleteFriendshipCommandValidator _validator = new();

        [Fact]
        public void Validator_Should_Have_Error_When_UserProfileId_Is_Null()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.Empty,
                FriendTag = "ValidTag"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteFriendshipCommand.Id));
        }

        [Fact]
        public void Validator_Should_Have_Error_When_UserProfileId_Is_Empty()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.Empty,
                FriendTag = "ValidTag"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteFriendshipCommand.Id));
        }

        [Fact]
        public void Validator_Should_Have_Error_When_FriendTag_Is_Null()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.NewGuid(),
                FriendTag = null
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteFriendshipCommand.FriendTag));
        }

        [Fact]
        public void Validator_Should_Have_Error_When_FriendTag_Is_Empty()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.NewGuid(),
                FriendTag = string.Empty
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteFriendshipCommand.FriendTag));
        }

        [Fact]
        public void Validator_Should_Have_Error_When_FriendTag_Is_Too_Long()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.NewGuid(),
                FriendTag = new string('A', 21) // 21 characters long
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteFriendshipCommand.FriendTag));
        }

        [Fact]
        public void Validator_Should_Pass_When_Command_Is_Valid()
        {
            // Arrange
            var command = new DeleteFriendshipCommand
            {
                Id = Guid.NewGuid(),
                FriendTag = "ValidTag"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
