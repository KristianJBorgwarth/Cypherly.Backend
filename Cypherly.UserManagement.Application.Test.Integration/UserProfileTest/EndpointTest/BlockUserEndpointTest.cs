using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Persistence.Context;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.EndpointTest;

public class BlockUserEndpointTest : IntegrationTestBase
{
    public BlockUserEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
    }
}