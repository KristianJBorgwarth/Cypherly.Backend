using System;
using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Domain.Common;
using FluentAssertions;
using Xunit;

namespace Cypherly.Authentication.Domain.Test.Unit.ServiceTest
{
    public class ClaimServiceTest
    {
        private readonly ClaimService _claimService;

        public ClaimServiceTest()
        {
            _claimService = new ClaimService();
        }

        [Fact]
        public void CreateClaim_ShouldReturnFailure_WhenClaimTypeIsEmpty()
        {
            // Arrange
            var claimType = string.Empty;

            // Act
            var result = _claimService.CreateClaim(claimType);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("The value cannot be empty: claimType");
        }

        [Fact]
        public void CreateClaim_ShouldReturnFailure_WhenClaimTypeIsTooLarge()
        {
            // Arrange
            var claimType = new string('a', 31); // Claim type exceeds the allowed limit of 30 characters

            // Act
            var result = _claimService.CreateClaim(claimType);

            // Assert
            result.Success.Should().BeFalse();
            result.Error.Message.Should().Contain("Value 'claimType' should not exceed 30.");
        }

        [Fact]
        public void CreateClaim_ShouldReturnSuccess_WhenClaimTypeIsValid()
        {
            // Arrange
            var claimType = "Admin";

            // Act
            var result = _claimService.CreateClaim(claimType);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ClaimType.Should().Be(claimType.ToLower()); // Claim type should be formatted to lowercase
        }

        [Fact]
        public void CreateClaim_ShouldReturnSuccess_WithCorrectlyFormattedClaimType()
        {
            // Arrange
            var claimType = "ADMIN";

            // Act
            var result = _claimService.CreateClaim(claimType);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.ClaimType.Should().Be("admin"); // Claim type should be formatted to lowercase
        }

        [Fact]
        public void CreateClaim_ShouldGenerateUniqueIdForNewClaim()
        {
            // Arrange
            var claimType = "User";

            // Act
            var result = _claimService.CreateClaim(claimType);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty(); // A unique GUID should be generated
        }
    }
}
