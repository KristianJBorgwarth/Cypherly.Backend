using Cypherly.Authentication.Persistence.Context;

namespace Cypherly.Authentication.Application.Test.Integration.Setup.Fixtures;

[CollectionDefinition("AuthenticationApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, AuthenticationDbContext>>
{

}