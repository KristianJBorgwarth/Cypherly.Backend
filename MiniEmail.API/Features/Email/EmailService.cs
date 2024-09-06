using FluentResults;
using MailKit.Net.Smtp;
using MimeKit;

namespace MinimalEmail.API.Features.Email;

public class EmailService(SmtpClient smtpClient, ILogger<EmailService> logger)
    : IEmailService
{
    private const string FromAddress = "noreply@cypherl.org";
    private const string SenderName = "Cypherly";

    public async Task<Result> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(SenderName, FromAddress));

            message.To.Add(new MailboxAddress(null, to));

            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            await smtpClient.SendAsync(message);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email");
            return Result.Fail(ex.Message);
        }
    }
}