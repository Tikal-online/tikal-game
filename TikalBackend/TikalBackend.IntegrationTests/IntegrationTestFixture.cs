using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

internal abstract class IntegrationTestFixture : TestContainerFixture
{
    private CustomWebApplicationFactory factory;

    protected HttpClient Client { get; private set; }

    [SetUp]
    public void Setup()
    {
        factory = new CustomWebApplicationFactory(DatabaseContainer.GetConnectionString());
        Client = factory.CreateDefaultClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        factory.Dispose();
    }
}