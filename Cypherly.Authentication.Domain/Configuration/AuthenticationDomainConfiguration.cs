using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Authentication.Domain.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Domain.Configuration;

public static class AuthenticationDomainConfiguration
{
    public static void AddAuthenticationDomain(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IClaimService, ClaimService>();
    }
}