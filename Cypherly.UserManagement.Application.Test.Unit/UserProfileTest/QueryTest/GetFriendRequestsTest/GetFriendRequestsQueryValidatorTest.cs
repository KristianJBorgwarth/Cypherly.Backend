using Xunit;
using FluentValidation.TestHelper;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetFriendRequestsTest
{
    public class GetFriendRequestsQueryValidatorTest
    {
        private readonly GetFriendRequestsQueryValidator _validator = new GetFriendRequestsQueryValidator();

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            // Arrange
            var query = new GetFriendRequestsQuery { UserId = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserId_Is_Provided()
        {
            // Arrange
            var query = new GetFriendRequestsQuery { UserId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }
    }
}