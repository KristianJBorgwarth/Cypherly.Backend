using FluentResults;

namespace MinimalEmail.API.Email;

public interface IEmailService
{
    Task<Result> SendEmailAsync(string to, string subject, string body);
}