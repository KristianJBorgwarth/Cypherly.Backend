using DotNet.Testcontainers.Builders;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalEmail.API.Email.Smtp;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace MiniEmail.API.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly IContainer _mailContainer = new ContainerBuilder()
        .WithImage("mailhog/mailhog:latest")
        .WithExposedPort(8025)
        .WithExposedPort(1025)
        .WithPortBinding(8025, 8025)
        .WithPortBinding(1025, 1025)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            #region SMTP Configuration

            var smtpSettings = new SmtpSettings
            {
                Host = "localhost",
                Port = 1025,
                Username = "noreply@cypherly.org",
                Password = "",
                FromAddress = "noreply@cypherly.org",
                UseSsl = false
            };

            services.Configure<SmtpSettings>(options =>
            {
                options.Host = smtpSettings.Host;
                options.Port = smtpSettings.Port;
                options.Username = smtpSettings.Username;
                options.Password = smtpSettings.Password;
                options.FromAddress = smtpSettings.FromAddress;
                options.UseSsl = smtpSettings.UseSsl;
            });
            #endregion

            #region RabbitMQ Configuration

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(TProgram).Assembly);
            });

            #endregion
        });
    }

    public async Task InitializeAsync()
    {
        await _mailContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mailContainer.DisposeAsync();
    }
}