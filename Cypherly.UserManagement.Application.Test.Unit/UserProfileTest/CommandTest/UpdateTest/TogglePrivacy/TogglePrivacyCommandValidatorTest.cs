using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.TogglePrivacy;

public class TogglePrivacyCommandValidatorTest
{
    private readonly TogglePrivacyCommandValidator _sut = new();
    
    [Fact]
    public void Handle_WhenIdIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new TogglePrivacyCommand
        {
            Id = Guid.Empty,
            IsPrivate = false
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(TogglePrivacyCommand.Id));
    }
    
    [Fact]
    public void Handle_WhenIdIsNotEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new TogglePrivacyCommand
        {
            Id = Guid.NewGuid(),
            IsPrivate = false
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
    
}
