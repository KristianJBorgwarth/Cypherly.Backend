using Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;
using Cypherly.Authentication.Application.Services.Authentication;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TestUtilities;
// ReSharper disable ClassNeverInstantiated.Global

namespace Cypherly.Authentication.Application.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram, TDbContext> : BaseIntegrationTestFactory<TProgram, TDbContext>
    where TProgram : class
    where TDbContext : DbContext
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            #region RabbitMq Configuration

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<CreateUserProfileRequest>(async cxt =>
                {
                    await cxt.RespondAsync(new CreateUserProfileResponse());
                });
            });

            #endregion

            #region Jwt Configuration

            services.RemoveAll(typeof(IConfigureOptions<JwtSettings>));

            var inMemorySettings = new Dictionary<string, string>()
            {
                { "Jwt:Secret", "SuperSecretJwtKeyForTestingOnly!@1234567890" },
                { "Jwt:Issuer", "Cypherly.Authentication.Test" },
                { "Jwt:Audience", "Test" },
                { "Jwt:TokenLifeTimeInMinutes", "20" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            #endregion
        });

    }
}