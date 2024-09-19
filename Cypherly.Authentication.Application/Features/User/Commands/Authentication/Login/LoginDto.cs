namespace Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;

public sealed record LoginDto
{
    public string JwtToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public DateTime RefreshTokenExpires { get; init; }
}