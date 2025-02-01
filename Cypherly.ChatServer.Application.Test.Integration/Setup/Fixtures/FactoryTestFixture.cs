using Cypherly.ChatServer.Persistence.Context;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Cypherly.ChatServer.Application.Test.Integration.Setup.Fixtures;

[CollectionDefinition("ChatServerApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, ChatServerDbContext>>
{

}