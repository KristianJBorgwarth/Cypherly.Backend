namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public sealed record GetUserProfileDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public string? DisplayName { get; init; }

    // hide constructor to enforce use of map method
    private GetUserProfileDto() { }

    public static GetUserProfileDto MapFrom(Domain.Aggregates.UserProfile userProfile, string? profilePictureUrl)
    {
        return new GetUserProfileDto()
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            UserTag = userProfile.UserTag.Tag,
            DisplayName = userProfile.DisplayName,
            ProfilePictureUrl = profilePictureUrl,
        };
    }
}

