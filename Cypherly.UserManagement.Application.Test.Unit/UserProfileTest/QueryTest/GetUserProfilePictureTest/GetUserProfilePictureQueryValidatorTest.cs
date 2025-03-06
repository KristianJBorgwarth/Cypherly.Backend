using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;
using FluentValidation.TestHelper;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfilePictureTest;

public class GetUserProfilePictureQueryValidatorTest
{
    private readonly GetUserProfilePictureQueryValidator _validator = new GetUserProfilePictureQueryValidator();

    [Fact]
    public void Should_Have_Error_When_ProfilePictureUrl_Is_Empty()
    {
        // Arrange
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = string.Empty };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProfilePictureUrl);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProfilePictureUrl_Is_Valid()
    {
        // Arrange
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = "https://example.com/profile.jpg" };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProfilePictureUrl);
    }
}