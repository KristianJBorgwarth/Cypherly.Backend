using Cypherly.API.Filters;
using Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TestUtilities.Authentication;

namespace TestUtilities;

public class BaseIntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

            #region Database Configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(),
                    b => b.MigrationsAssembly(typeof(TDbContext).Assembly.FullName))
                    .UseLazyLoadingProxies();
            });

            #endregion


            // Mock out authentication and authorization for testing
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(_ => true));
                options.AddPolicy("User", policy => policy.RequireAssertion(_ => true));
            });
            
            // Mock out ValidateUserIdFilter
            // Remove the existing ValidateUserIdFilter registration
            var actionFilterDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IValidateUserIdFilter));

            if (actionFilterDescriptor != null)
            {
                services.Remove(actionFilterDescriptor);
            }

            // Replace with a mock or NoOp implementation
            services.AddScoped<IValidateUserIdFilter, MockValidateUserIdIdFilter>();
        });
    }

    public virtual async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new virtual async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}