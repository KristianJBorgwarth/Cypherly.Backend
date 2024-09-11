using Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
        });

    }
}