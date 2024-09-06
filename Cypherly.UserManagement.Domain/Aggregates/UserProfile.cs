using Cypherly.Domain.Common;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Aggregates;

public partial class UserProfile : AggregateRoot
{
    public string Username { get; private set; } = null!;
    public UserTag UserTag { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    public UserProfile() : base(Guid.Empty){} // For EF Core

    public UserProfile(Guid id, string username, UserTag userUserTag) : base(id)
    {
        Username = username;
        UserTag = userUserTag;
    }
    public void SetProfilePictureUrl(string profilePictureUrl)
    {
        ProfilePictureUrl = profilePictureUrl;
    }

    public Result SetDisplayName(string displayName)
    {
        if(displayName.Length < 3)
            return Result.Fail(Errors.General.ValueTooSmall(nameof(displayName), 3));
        if(displayName.Length > 20)
            return Result.Fail(Errors.General.ValueTooLarge(nameof(displayName), 20));
        if (!DisplayNameRegex().IsMatch(displayName))
            return Result.Fail(Errors.General.UnexpectedValue(nameof(displayName)));

        DisplayName = displayName;
        return Result.Ok();
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9]*$")]
    private static partial System.Text.RegularExpressions.Regex DisplayNameRegex();
}