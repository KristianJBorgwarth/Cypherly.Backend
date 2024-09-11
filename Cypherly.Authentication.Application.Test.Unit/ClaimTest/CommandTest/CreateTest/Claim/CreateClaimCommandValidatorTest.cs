using Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;
using FluentAssertions;
using Xunit;

namespace Cypherly.Authentication.Application.Test.Unit.ClaimTest.CommandTest.CreateTest.Claim
{
    public class CreateClaimCommandValidatorTest
    {
        private readonly CreateClaimCommandValidator _sut;

        public CreateClaimCommandValidatorTest()
        {
            _sut = new CreateClaimCommandValidator();
        }

        [Fact]
        public void Validator_ShouldReturnError_WhenClaimTypeIsEmpty()
        {
            // Arrange
            var command = new CreateClaimCommand
            {
                ClaimType = string.Empty
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Contain("The value cannot be empty: ClaimType");
        }

        [Fact]
        public void Validator_ShouldPass_WhenClaimTypeIsValid()
        {
            // Arrange
            var command = new CreateClaimCommand
            {
                ClaimType = "Admin"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validator_ShouldFail_WhenClaimTypeIsNull()
        {
            // Arrange
            var command = new CreateClaimCommand
            {
                ClaimType = null // ClaimType is null
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Contain("The value cannot be empty: ClaimType");
        }
    }
}
