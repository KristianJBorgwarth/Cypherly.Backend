namespace MinimalEmail.API.Configuration;

public sealed class SmtpSettings
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FromAddress { get; init; }
    public bool UseSsl { get; init; } = true;
}