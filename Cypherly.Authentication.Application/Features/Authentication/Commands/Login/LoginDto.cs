namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public sealed record LoginDto
{
    public Guid Id { get; init; } 
    public bool IsVerified { get; init; }
    public string? JwtToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? RefreshTokenExpires { get; init; }
}