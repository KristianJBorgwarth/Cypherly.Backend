using System.Net.Mail;
using FluentAssertions;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using MinimalEmail.API.Email;
using MinimalEmail.API.Email.Smtp;

namespace MiniEmail.API.Test.Unit;

public class EmailServiceTest
{
    private readonly ILogger<EmailService> _logger = A.Fake<ILogger<EmailService>>();
    private readonly ISmtpClient _smtpClient = A.Fake<ISmtpClient>();

    [Fact]
    public async Task SendEmailAsync_Should_Send_Email_When_Valid_Inputs_Are_Provided()
    {
        // Arrange
        var emailService = new EmailService(_logger, _smtpClient);
        const string to = "test@example.com";
        const string subject = "Test Subject";
        const string body = "<h1>Hello World</h1>";

        // Act
        var result = await emailService.SendEmailAsync(to, subject, body);

        // Assert
        result.IsSuccess.Should().BeTrue();
        A.CallTo(() => _smtpClient.SendMailAsync(A<MailMessage>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendEmailAsync_Should_Return_Failure_And_Log_Error_When_Exception_Is_Thrown()
    {
        // Arrange
        var emailService = new EmailService(_logger, _smtpClient);
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "<h1>Hello World</h1>";

        // Simulate SmtpClient throwing an exception
        A.CallTo(() => _smtpClient.SendMailAsync(A<MailMessage>.Ignored))
            .Throws(new Exception("SMTP error"));

        // Act
        var result = await emailService.SendEmailAsync(to, subject, body);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("SMTP error");
    }
}
