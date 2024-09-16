using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.DisplayName
{
    public class UpdateUserProfileDisplayNameCommandValidatorTest
    {
        private readonly UpdateUserProfileDisplayNameCommandValidator _sut = new();

        [Fact]
        public void Validate_ShouldHaveError_WhenUserProfileIdIsNull()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                UserProfileId = Guid.Empty,
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be(Errors.General.ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.UserProfileId)).Message);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenDisplayNameIsNull()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                UserProfileId = Guid.NewGuid(),
                DisplayName = null
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be(Errors.General.ValueIsRequired(nameof(UpdateUserProfileDisplayNameCommand.DisplayName)).Message);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                UserProfileId = Guid.NewGuid(),
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenUserProfileIdIsEmpty()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                UserProfileId = Guid.Empty,
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be(Errors.General.ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.UserProfileId)).Message);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenDisplayNameIsEmpty()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                UserProfileId = Guid.NewGuid(),
                DisplayName = ""
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be(Errors.General.ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.DisplayName)).Message);
        }
    }
}
