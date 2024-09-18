using Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;
using FluentAssertions;
using Xunit;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.UpdateTest.ResendAccountVerificationCommand
{
    public class ResendAccountVerificationCommandValidatorTest
    {
        private readonly ResendAccountVerificationCommandValidator _sut = new();

        [Fact]
        public void Validate_ShouldHaveError_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
            {
                UserId = Guid.Empty
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.UserId));
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenUserIdIsValid()
        {
            // Arrange
            var command = new Features.User.Commands.Update.ResendVerificationCode.ResendAccountVerificationCommand
            {
                UserId = Guid.NewGuid()
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}