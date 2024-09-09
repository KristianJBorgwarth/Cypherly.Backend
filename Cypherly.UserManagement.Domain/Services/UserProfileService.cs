using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Services;

public interface IUserProfileService
{
    UserProfile CreateUserProfile(Guid userId, string username);
}
public class UserProfileService : IUserProfileService
{
    public UserProfile CreateUserProfile(Guid userId, string username)
    {
        var tag = UserTag.Create(username);
        return new(userId, username, tag);
    }
}