using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace MinimalEmail.API.Email.Smtp;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(IOptions<SmtpSettings> settings)
    {
        var smtpSettings = settings.Value;
        _smtpClient = new()
        {
            Host = smtpSettings.Host,
            Port = smtpSettings.Port,
            Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
            EnableSsl = smtpSettings.UseSsl
        };
    }

    public Task SendMailAsync(MailMessage mailMessage)
    {
        return _smtpClient.SendMailAsync(mailMessage);
    }
}