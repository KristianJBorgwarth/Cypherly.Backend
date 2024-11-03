using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Domain.Configuration;

public static class UserManagementDomainConfiguration
{
    public static void AddUserManagementDomainServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserProfileService, UserProfileService>();
        serviceCollection.AddScoped<IFriendshipService, FriendshipService>();
    }
}