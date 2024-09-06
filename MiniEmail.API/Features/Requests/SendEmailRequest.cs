namespace MinimalEmail.API.Features.Requests;

public sealed record SendEmailRequest
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}