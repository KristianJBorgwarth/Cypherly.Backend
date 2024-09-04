using FluentResults;

namespace MinimalEmail.API.Features.Email;

public class EmailService : IEmailService
{
    public Task<Result> SendEmailAsync(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }
}