namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public class GetBlockedUserProfilesDto
{
    public string Username { get; private init; } = null!;
    public string Tag { get; private init; } = null!;
    public string? DisplayName { get; private init; }
    
    private GetBlockedUserProfilesDto() { } // Hide the constructor
    
    public static GetBlockedUserProfilesDto MapFrom(Domain.Aggregates.UserProfile userProfile)
    {
        return new GetBlockedUserProfilesDto
        {
            Username = userProfile.Username,
            Tag = userProfile.UserTag.Tag,
            DisplayName = userProfile.DisplayName
        };
    }
}