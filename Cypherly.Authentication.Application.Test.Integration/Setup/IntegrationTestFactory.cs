using Cypherly.Authentication.Application.Features.Authentication.Token;
using Cypherly.Authentication.Redis.Configuration;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using DotNet.Testcontainers.Builders;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TestUtilities;
using DotNet.Testcontainers.Containers;

// ReSharper disable ClassNeverInstantiated.Global

namespace Cypherly.Authentication.Application.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram, TDbContext> : BaseIntegrationTestFactory<TProgram, TDbContext>
    where TProgram : class
    where TDbContext : DbContext
{
    private readonly IContainer _valkeyContainer = new ContainerBuilder()
        .WithImage("valkey/valkey:latest")
        .WithEnvironment("ALLOW_EMPTY_PASSWORD", "yes")
        .WithExposedPort(6974)
        .WithPortBinding(6974, 6379)
        .WithCleanUp(true)
        .Build();

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
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            #endregion

            #region Valkey Configuration

            services.Configure<ValkeySettings>(options =>
            {
                options.Host = "localhost";  // The test container's host
                options.Port = 6974;        // The mapped port for the Redis container
            });

            #endregion
        });
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _valkeyContainer.StartAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _valkeyContainer.StopAsync();
    }
}