using MassTransit.Testing;
using MiniEmail.API.Test.Integration.Setup.Helpers;

namespace MiniEmail.API.Test.Integration.Setup;

[Collection("MiniEmailAPI")]
public class IntegrationTestBase : IDisposable
{
    protected readonly HttpClient Client;
    protected readonly ITestHarness Harness;
    protected readonly MailHogHelper MailHogHelper;

    public IntegrationTestBase(IntegrationTestFactory<Program> factory)
    {
        Harness = factory.Services.GetTestHarness();
        Client = factory.CreateClient();
        Harness.Start();
        MailHogHelper = new();
    }

    public void Dispose()
    {
        Harness.Stop();
        Client.Dispose();
        MailHogHelper.ClearMailHogMessage().Wait();
    }
}