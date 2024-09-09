using System.Net.Mail;
using FluentResults;
using MinimalEmail.API.Email.Smtp;

namespace MinimalEmail.API.Email;

public class EmailService(ILogger<EmailService> logger, ISmtpClient smtpClient) : IEmailService
{
    private const string FromAddress = "noreply@cypherly.org";

    public async Task<Result> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var mailMessage = new MailMessage(FromAddress, to, subject, body)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email");
            return Result.Fail(ex.Message);
        }
    }
}