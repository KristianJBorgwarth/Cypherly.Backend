using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using FluentValidation.TestHelper;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetFriends
{
    public class GetFriendsQueryValidatorTest
    {
        private readonly GetFriendsQueryValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Null()
        {
            // Arrange
            var query = new GetFriendsQuery { UserProfileId = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserProfileId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.UserProfileId)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Empty()
        {
            // Arrange
            var query = new GetFriendsQuery { UserProfileId = Guid.Empty }; // Empty Guid

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserProfileId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.UserProfileId)).Message);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserProfileId_Is_Valid()
        {
            // Arrange
            var query = new GetFriendsQuery { UserProfileId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserProfileId);
        }
    }
}