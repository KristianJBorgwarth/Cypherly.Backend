using Cypherly.Domain.Common;
using Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;
using FluentValidation.TestHelper;
using Xunit;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.QueryTest.GetUserDevices
{
    public class GetUserDevicesQueryValidatorTest
    {
        private readonly GetUserDevicesQueryValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Null()
        {
            // Arrange
            var query = new GetUserDevicesQuery() { UserId = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetUserDevicesQuery.UserId)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Empty()
        {
            // Arrange
            var query = new GetUserDevicesQuery { UserId = Guid.Empty }; // Empty Guid

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetUserDevicesQuery.UserId)).Message);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserProfileId_Is_Valid()
        {
            // Arrange
            var query = new GetUserDevicesQuery { UserId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }
    }
}