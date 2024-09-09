using Microsoft.EntityFrameworkCore;
using TestUtilities;
// ReSharper disable ClassNeverInstantiated.Global

namespace Cypherly.Authentication.Application.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram, TDbContext> : BaseIntegrationTestFactory<TProgram, TDbContext>
    where TProgram : class
    where TDbContext : DbContext
{

}