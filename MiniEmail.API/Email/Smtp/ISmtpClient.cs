using System.Net.Mail;

namespace MinimalEmail.API.Email.Smtp;

public interface ISmtpClient
{
    Task SendMailAsync(MailMessage mailMessage);
}