namespace Cypherly.UserManagement.Bucket.Configuration;

public sealed class MinioSettings
{
    public required string Host { get; init; } = null!;
    public required string User { get; init; } = null!;
    public required string ProfilePictureBucket { get; init; } = null!;
    public required string Password { get; init; } = null!;
}