using Identity.IntegrationTests.Utils;
using NUnit.Framework;

namespace Identity.IntegrationTests;

public abstract class IntegrationTestFixture : TestContainerFixture
{
    private CustomWebApplicationFactory factory;

    protected HttpClient Client { get; private set; }

    [SetUp]
    public void Setup()
    {
        factory = new CustomWebApplicationFactory(DatabaseContainer.GetConnectionString());
        Client = factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        factory.Dispose();
    }
}