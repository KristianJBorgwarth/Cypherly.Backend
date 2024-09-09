using FluentValidation.TestHelper;
using MinimalEmail.API.Features.Requests;
using Xunit;

namespace MiniEmail.API.Test.Unit;

public class SendEmailRequestValidatorTests
{
    private readonly SendEmailRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_To_Is_Empty()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "",
            Subject = "Test Subject",
            Body = "This is a test email."
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.To)
              .WithErrorMessage("Recipient email address is required.");
    }

    [Fact]
    public void Should_Have_Error_When_To_Is_Invalid_Email()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "invalid-email",
            Subject = "Test Subject",
            Body = "This is a test email."
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.To)
              .WithErrorMessage("Recipient email address is invalid.");
    }

    [Fact]
    public void Should_Have_Error_When_Subject_Is_Empty()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "test@example.com",
            Subject = "",
            Body = "This is a test email."
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Subject)
              .WithErrorMessage("Email subject is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Subject_Is_Too_Long()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "test@example.com",
            Subject = new string('A', 101), // Subject exceeds the max length of 100 characters
            Body = "This is a test email."
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Subject)
              .WithErrorMessage("Email subject is too long.");
    }

    [Fact]
    public void Should_Have_Error_When_Body_Is_Empty()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = ""
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Body)
              .WithErrorMessage("Email body is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var model = new SendEmailRequest
        {
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "This is a test email."
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
        result.ShouldNotHaveValidationErrorFor(x => x.Subject);
        result.ShouldNotHaveValidationErrorFor(x => x.Body);
    }
}
