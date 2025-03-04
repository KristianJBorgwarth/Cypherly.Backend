using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// ReSharper disable ConvertToUsingDeclaration

namespace Cypherly.Persistence.Configuration;

public static class ApplyMigrations
{
    public static void ApplyPendingMigrations<TDbContext>(this IServiceProvider provider)
        where TDbContext : DbContext
    {
        using (var scope = provider.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<Logger<IServiceProvider>>();

            try
            {
                var dbContext = services.GetRequiredService<TDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying migrations...");
                    dbContext.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("No pending migrations found");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while attempting to apply migration to the database");
                throw;
            }
        }
    }
}
