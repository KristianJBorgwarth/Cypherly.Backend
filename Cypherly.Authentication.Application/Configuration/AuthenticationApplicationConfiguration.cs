using System.Reflection;
using Cypherly.Application.Configuration;
using Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;
using Cypherly.Authentication.Application.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Application.Configuration;

public static class AuthenticationApplicationConfiguration
{
    public static void AddAuthenticationApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddApplication(assembly);
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IVerifyNonceService, VerifyNonceService>();
    }
}