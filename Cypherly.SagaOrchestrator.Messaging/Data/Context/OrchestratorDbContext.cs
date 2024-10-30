using Cypherly.SagaOrchestrator.Messaging.Data.ModelConfigurations;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.SagaOrchestrator.Messaging.Data.Context;

public sealed class OrchestratorDbContext : SagaDbContext
{
    public OrchestratorDbContext(DbContextOptions<OrchestratorDbContext> options) : base(options) { }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new UserDeleteSagaMap();
        }
    }
}