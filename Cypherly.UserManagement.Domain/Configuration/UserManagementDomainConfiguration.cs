using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Domain.Configuration;

public static class UserManagementDomainConfiguration
{
    public static void AddUserManagementDomainServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserProfileLifecycleService, UserProfileLifecycleLifecycleService>();
        serviceCollection.AddScoped<IFriendshipService, FriendshipService>();
        serviceCollection.AddScoped<IUserBlockingService, UserBlockingService>();
    }
}