using Cypherly.UserManagement.Persistence.Context;

namespace Cypherly.UserManagement.Application.Test.Integration.Setup.Fixtures;

[CollectionDefinition("UserManagementApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, UserManagementDbContext>>
{

}