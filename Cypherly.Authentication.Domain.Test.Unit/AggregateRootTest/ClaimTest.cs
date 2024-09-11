using System;
using System.Collections.Generic;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Cypherly.Authentication.Domain.Test.Unit.AggregateRootTest
{
    public class ClaimTest
    {
        [Fact]
        public void AddUserClaim_ShouldAddUserClaimToCollection()
        {
            // Arrange
            var claimId = Guid.NewGuid();
            var claim = new Claim(claimId, "Admin");
            var userClaim = new UserClaim(Guid.NewGuid(), Guid.NewGuid(), claimId);

            // Act
            claim.AddUserClaim(userClaim);

            // Assert
            claim.UserClaims.Should().Contain(userClaim);
            claim.UserClaims.Should().HaveCount(1);
        }
        
        [Fact]
        public void RemoveUserClaim_ShouldRemoveUserClaimFromCollection()
        {
            // Arrange
            var claimId = Guid.NewGuid();
            var claim = new Claim(claimId, "Admin");
            var userClaim = new UserClaim(Guid.NewGuid(), Guid.NewGuid(), claimId);

            claim.AddUserClaim(userClaim);

            // Act
            claim.RemoveUserClaim(userClaim);

            // Assert
            claim.UserClaims.Should().NotContain(userClaim);
            claim.UserClaims.Should().BeEmpty();
        }

        [Fact]
        public void RemoveUserClaim_ShouldNotThrowIfUserClaimDoesNotExist()
        {
            // Arrange
            var claimId = Guid.NewGuid();
            var claim = new Claim(claimId, "Admin");
            var userClaim = new UserClaim(Guid.NewGuid(), Guid.NewGuid(), claimId);

            // Act
            Action act = () => claim.RemoveUserClaim(userClaim);

            // Assert
            act.Should().NotThrow();
            claim.UserClaims.Should().BeEmpty();
        }

        [Fact]
        public void UserClaims_ShouldBeReadOnlyCollection()
        {
            // Arrange
            var claimId = Guid.NewGuid();
            var claim = new Claim(claimId, "Admin");

            // Act
            var userClaims = claim.UserClaims;

            // Assert
            userClaims.Should().BeAssignableTo<IReadOnlyCollection<UserClaim>>();
        }

        [Fact]
        public void ClaimType_ShouldBeSetCorrectly()
        {
            // Arrange
            var claimId = Guid.NewGuid();
            var claimType = "Admin";
            var claim = new Claim(claimId, claimType);

            // Act & Assert
            claim.ClaimType.Should().Be(claimType);
        }
    }
}
