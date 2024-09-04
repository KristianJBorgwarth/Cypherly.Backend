using FluentResults;

namespace MinimalEmail.API.Features.Email;

public interface IEmailService
{
    Task<Result> SendEmailAsync(string to, string subject, string body);
}