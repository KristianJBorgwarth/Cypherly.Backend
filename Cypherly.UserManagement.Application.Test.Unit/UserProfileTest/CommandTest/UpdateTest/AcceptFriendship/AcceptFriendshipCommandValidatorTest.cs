using FluentAssertions;
using Xunit;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using System;
using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.AcceptFriendship
{
    public class AcceptFriendshipCommandValidatorTest
    {
        private readonly AcceptFriendshipCommandValidator _sut = new AcceptFriendshipCommandValidator();

        [Fact]
        public void Validator_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                UserId = Guid.NewGuid(),
                FriendTag = "ValidFriendTag"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                UserId = Guid.Empty, // Invalid UserId
                FriendTag = "ValidFriendTag"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.UserId) && e.ErrorMessage == Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.UserId)).Message);
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenFriendTagIsNull()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                UserId = Guid.NewGuid(),
                FriendTag = null // Invalid FriendTag
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.FriendTag) && e.ErrorMessage == Errors.General.ValueIsRequired(nameof(AcceptFriendshipCommand.FriendTag)).Message);
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenFriendTagIsEmpty()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                UserId = Guid.NewGuid(),
                FriendTag = "" // Invalid FriendTag
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.FriendTag) && e.ErrorMessage == Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.FriendTag)).Message);
        }
    }
}