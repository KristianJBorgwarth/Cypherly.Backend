using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;
using FluentAssertions;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.UnblockUser;

public class UnblockUserCommandValidatorTest
{
    private readonly UnblockUserCommandValidator _sut = new();
    
    [Fact]
    public void Handle_WhenIdIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new UnblockUserCommand
        {
            Id = Guid.Empty,
            Tag = "tag"
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UnblockUserCommand.Id));
    }
    
    [Fact]
    public void Handle_WhenTagIsNull_ShouldReturnValidationError()
    {
        // Arrange
        var command = new UnblockUserCommand
        {
            Id = Guid.NewGuid(),
            Tag = null
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public void Handle_WhenTagIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new UnblockUserCommand
        {
            Id = Guid.NewGuid(),
            Tag = string.Empty
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UnblockUserCommand.Tag));
    }
    
    [Fact]
    public void Handle_WhenTagIsWhitespace_ShouldReturnValidationError()
    {
        // Arrange
        var command = new UnblockUserCommand
        {
            Id = Guid.NewGuid(),
            Tag = " "
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UnblockUserCommand.Tag));
    }
    
    [Fact]
    public void Handle_WhenCommandIsValid_ShouldReturnNoValidationErrors()
    {
        // Arrange
        var command = new UnblockUserCommand
        {
            Id = Guid.NewGuid(),
            Tag = "tag"
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}